using System.Text;
using System.Text.Json;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace LinguiCards.Infrastructure.Integration.OpenAI;

public class OpenAIClient: IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIClient(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<string> GetChatResponseAsync(string prompt)
    {
        var requestBody = new
        {
            model = "gpt-4.1-mini-2025-04-14",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Headers =
            {
                { "Authorization", $"Bearer {_apiKey}" }
            },
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(result);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }
}
