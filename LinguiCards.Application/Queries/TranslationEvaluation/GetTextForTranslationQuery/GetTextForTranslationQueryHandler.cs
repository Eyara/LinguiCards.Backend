using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTextForTranslationQuery;

public class GetTextForTranslationQueryHandler : IRequestHandler<GetTextForTranslationQuery, string>
{
    private readonly IOpenAIService _openAiService;

    public GetTextForTranslationQueryHandler(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<string> Handle(GetTextForTranslationQuery request,
        CancellationToken cancellationToken)
    {
        return await _openAiService.GetChatResponseAsync(
            TranslationPrompts.GetStartGamePrompt("Латынь", request.Length, request.Level, request.Topic));
    }
}