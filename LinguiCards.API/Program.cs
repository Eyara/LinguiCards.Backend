using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using LinguiCards.Application.Commands.User.AddUserCommand;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Options;
using LinguiCards.Application.Middlewares;
using LinguiCards.AutoMapper.Profiles;
using LinguiCards.Infrastructure;
using LinguiCards.Infrastructure.Integration.Caching;
using LinguiCards.Infrastructure.Integration.OpenAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            ClockSkew = TimeSpan.Zero // Disable default 5 minutes
        };
    });

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddDbContext<LinguiCardsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AddUserCommand).GetTypeInfo().Assembly));

builder.Services.AddAutoMapper(typeof(WordProfile).Assembly);

builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<ProxyOptions>(builder.Configuration.GetSection("Proxy"));

builder.Services.AddHttpClient<IOpenAIService, OpenAIClient>()
    .ConfigurePrimaryHttpMessageHandler(sp =>
    {
        var proxyOptions = sp.GetRequiredService<IOptions<ProxyOptions>>().Value;

        var proxy = new WebProxy(proxyOptions.Address)
        {
            Credentials = new NetworkCredential(proxyOptions.Username, proxyOptions.Password)
        };

        return new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true
        };
    });

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

    // Apply the security scheme globally to all endpoints
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
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
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


app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LinguiCardsDbContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}

app.Run();