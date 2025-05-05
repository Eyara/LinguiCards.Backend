using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Integration;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTranslationEvaluationQuery;

public class GetTranslationEvaluationQueryHandler : IRequestHandler<GetTranslationEvaluationQuery, TranslationEvaluationDTO>
{
    private readonly IOpenAIService _openAiService;

    public GetTranslationEvaluationQueryHandler(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<TranslationEvaluationDTO> Handle(GetTranslationEvaluationQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _openAiService.GetChatResponseAsync(
            TranslationPrompts.GetEvaluationTranslationPrompt("Латынь", request.Level, request.OriginalText, request.Translation));
        
        Console.WriteLine(response);

        return ParseEvaluationOpenAI.ParseEvaluation(response);
    }
}