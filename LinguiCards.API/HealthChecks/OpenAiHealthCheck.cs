using System.Net.Http.Headers;
using LinguiCards.Application.Common.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinguiCards.HealthChecks;

public class OpenAiHealthCheck : IHealthCheck
{
    public const string HttpClientName = "OpenAIHealthCheck";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenAiHealthCheck> _logger;
    private readonly OpenAIOptions _openAiOptions;
    private readonly OpenAiHealthCheckOptions _healthCheckOptions;

    public OpenAiHealthCheck(
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAiHealthCheck> logger,
        IOptions<OpenAIOptions> openAiOptions,
        IOptions<OpenAiHealthCheckOptions> healthCheckOptions)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _openAiOptions = openAiOptions.Value;
        _healthCheckOptions = healthCheckOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_openAiOptions.ApiKey))
        {
            _logger.LogWarning("OpenAI health check is unhealthy because API key is not configured.");
            return HealthCheckResult.Unhealthy("OpenAI API key is not configured.");
        }

        var timeoutSeconds = Math.Clamp(_healthCheckOptions.TimeoutSeconds, 1, 60);
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            var request = new HttpRequestMessage(HttpMethod.Get, _healthCheckOptions.EndpointPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiOptions.ApiKey);

            var response = await httpClient.SendAsync(request, timeoutCts.Token);
            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy("OpenAI integration is reachable.");

            var responseBody = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            var bodyPreview = responseBody.Length > 500 ? responseBody[..500] : responseBody;

            var reason = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? "Unauthorized (invalid or expired API key)."
                : $"OpenAI returned {(int)response.StatusCode} ({response.StatusCode}).";

            _logger.LogWarning(
                "OpenAI health check is unhealthy. StatusCode: {StatusCode}. Endpoint: {EndpointPath}. Response: {ResponseBody}",
                (int)response.StatusCode,
                _healthCheckOptions.EndpointPath,
                bodyPreview);

            return HealthCheckResult.Unhealthy(reason);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "OpenAI health check timed out after {TimeoutSeconds} seconds. Endpoint: {EndpointPath}",
                timeoutSeconds,
                _healthCheckOptions.EndpointPath);
            return HealthCheckResult.Unhealthy($"OpenAI health probe timed out after {timeoutSeconds} seconds.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "OpenAI health check request failed. Endpoint: {EndpointPath}",
                _healthCheckOptions.EndpointPath);
            return HealthCheckResult.Unhealthy($"OpenAI request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "OpenAI health check failed with unexpected error. Endpoint: {EndpointPath}",
                _healthCheckOptions.EndpointPath);
            return HealthCheckResult.Unhealthy($"Unexpected OpenAI health-check error: {ex.Message}");
        }
    }
}
