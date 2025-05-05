namespace LinguiCards.Application.Common.Interfaces;

public interface IOpenAIService
{
    Task<string> GetChatResponseAsync(string prompt);
}