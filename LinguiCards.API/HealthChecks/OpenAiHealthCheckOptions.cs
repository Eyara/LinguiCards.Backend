namespace LinguiCards.HealthChecks;

public class OpenAiHealthCheckOptions
{
    public const string SectionPath = "HealthChecks:OpenAI";

    public int TimeoutSeconds { get; set; } = 8;

    public string EndpointPath { get; set; } = "/v1/models?limit=1";
}
