using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using AutoMapper;
using LinguiCards.Application.Commands.User.AddUserCommand;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Options;
using LinguiCards.Application.Middlewares;
using LinguiCards.AutoMapper.Profiles;
using LinguiCards.HealthChecks;
using LinguiCards.Helpers;
using LinguiCards.Infrastructure;
using LinguiCards.Infrastructure.Integration.Caching;
using LinguiCards.Infrastructure.Integration.OpenAI;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

if (builder.Environment.IsDevelopment()) builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddDbContext<LinguiCardsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AddUserCommand).GetTypeInfo().Assembly));

builder.Services.AddAutoMapper(typeof(WordProfile).Assembly);

builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<ProxyOptions>(builder.Configuration.GetSection("Proxy"));
builder.Services.Configure<OpenAiHealthCheckOptions>(
    builder.Configuration.GetSection(OpenAiHealthCheckOptions.SectionPath));

builder.Services.AddHttpClient<IOpenAIService, OpenAIClient>()
    .ConfigurePrimaryHttpMessageHandler(CreateProxySocketsHandler);
builder.Services.AddHttpClient(OpenAiHealthCheck.HttpClientName, client => { client.BaseAddress = new Uri("https://api.openai.com"); })
    .ConfigurePrimaryHttpMessageHandler(CreateProxySocketsHandler);

builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
    var config = ConfigurationOptions.Parse(redisOptions.GetConnectionString());
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token in the text input below. Example: 'Bearer 12345abcdef'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<OpenAiHealthCheck>(
        "openai",
        HealthStatus.Unhealthy,
        new[] { "external", "openai" });
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseRouting();
app.UseHttpMetrics();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration.TotalMilliseconds,
                    error = entry.Value.Exception?.Message,
                    data = entry.Value.Data
                })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});
app.MapMetrics("/metrics");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LinguiCardsDbContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}

app.Run();

static SocketsHttpHandler CreateProxySocketsHandler(IServiceProvider sp)
{
    var proxyOptions = sp.GetRequiredService<IOptions<ProxyOptions>>().Value;
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("OpenAI.Proxy");

    var hasProxyB = !string.IsNullOrWhiteSpace(proxyOptions.AddressB?.Host);
    var hasProxyA = !string.IsNullOrWhiteSpace(proxyOptions.AddressA?.Host);

    if (proxyOptions.ChainLength <= 0 || !hasProxyB || (proxyOptions.ChainLength == 2 && !hasProxyA))
    {
        logger.LogInformation(
            "OpenAI HTTP handler is using direct connection. ChainLength: {ChainLength}, HasProxyA: {HasProxyA}, HasProxyB: {HasProxyB}",
            proxyOptions.ChainLength,
            hasProxyA,
            hasProxyB);
        return new SocketsHttpHandler();
    }

    logger.LogInformation(
        "OpenAI HTTP handler is using proxy chain. ChainLength: {ChainLength}, ProxyAHost: {ProxyAHost}, ProxyBHost: {ProxyBHost}",
        proxyOptions.ChainLength,
        hasProxyA ? proxyOptions.AddressA.Host : "(not set)",
        hasProxyB ? proxyOptions.AddressB.Host : "(not set)");

    return new SocketsHttpHandler
    {
        ConnectCallback = async (context, cancellationToken) =>
        {
            var proxyB = proxyOptions.AddressB;
            var targetHost = context.DnsEndPoint.Host;
            var targetPort = context.DnsEndPoint.Port;

            switch (proxyOptions.ChainLength)
            {
                case 1:
                {
                    logger.LogInformation(
                        "OpenAI request proxy route: single proxy {ProxyHost}:{ProxyPort} -> {TargetHost}:{TargetPort}",
                        proxyB.Host,
                        proxyB.Port,
                        targetHost,
                        targetPort);

                    var tcp = new TcpClient();
                    await tcp.ConnectAsync(proxyB.Host, proxyB.Port, cancellationToken);
                    var stream = tcp.GetStream();
                    await ProxyChainingHelper.SendConnect(stream, targetHost, targetPort, proxyB.Username,
                        proxyB.Password, cancellationToken);
                    return stream;
                }
                case 2:
                {
                    var proxyA = proxyOptions.AddressA;
                    logger.LogInformation(
                        "OpenAI request proxy route: {ProxyAHost}:{ProxyAPort} -> {ProxyBHost}:{ProxyBPort} -> {TargetHost}:{TargetPort}",
                        proxyA.Host,
                        proxyA.Port,
                        proxyB.Host,
                        proxyB.Port,
                        targetHost,
                        targetPort);

                    var tcp = new TcpClient();
                    await tcp.ConnectAsync(proxyA.Host, proxyA.Port, cancellationToken);
                    var stream = tcp.GetStream();

                    await ProxyChainingHelper.SendConnect(stream, proxyB.Host, proxyB.Port, proxyA.Username,
                        proxyA.Password, cancellationToken);
                    await ProxyChainingHelper.SendConnect(stream, targetHost, targetPort, proxyB.Username,
                        proxyB.Password, cancellationToken);
                    return stream;
                }
                default:
                    throw new InvalidOperationException(
                        $"Invalid Proxy:ChainLength value '{proxyOptions.ChainLength}'. Supported values are 1 or 2.");
            }
        }
    };
}