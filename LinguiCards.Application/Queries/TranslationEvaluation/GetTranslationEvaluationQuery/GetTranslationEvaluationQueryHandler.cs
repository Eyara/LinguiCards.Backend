using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Integration;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTranslationEvaluationQuery;

public class GetTranslationEvaluationQueryHandler : IRequestHandler<GetTranslationEvaluationQuery, TranslationEvaluationDTO>
{
    private readonly IOpenAIService _openAiService;
    private readonly IUsersRepository _usersRepository;

    public GetTranslationEvaluationQueryHandler(IOpenAIService openAiService, IUsersRepository usersRepository)
    {
        _openAiService = openAiService;
        _usersRepository = usersRepository;
    }

    public async Task<TranslationEvaluationDTO> Handle(GetTranslationEvaluationQuery request,
        CancellationToken cancellationToken)
    {
        var user = await  _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        
        if (user == null)
        {
            throw new UserNotFoundException();
        }
        
        var response = await _openAiService.GetChatResponseAsync(
            TranslationPrompts.GetEvaluationTranslationPrompt("Латынь", request.Level, request.OriginalText, request.Translation));

        var parsedEvaluation = ParseEvaluationOpenAI.ParseEvaluation(response);
        var wordList = request.OriginalText.Split(' ').ToList();

        await UpdateXpLevel(user, LearningSettings.LanguageLevelToNumber(request.Level), wordList.Count,
            parsedEvaluation.Accuracy, cancellationToken);

        return parsedEvaluation;
    }
    
    private async Task UpdateXpLevel(Domain.Entities.User user, int level, int wordCount, double accuracyPercent, CancellationToken token)
    {
        var requiredXp = CalculatorXP.CalculateXpRequired(user.Level);

        accuracyPercent = Math.Clamp(accuracyPercent, 0, 100);
        var newXp = LearningSettings.SuccessXpStep * level * wordCount * (accuracyPercent / 100.0) + user.XP;
        
        var newLevel = user.Level;

        if (newXp >= requiredXp)
        {
            newLevel++;
            newXp -= requiredXp;
        }

        await _usersRepository.UpdateXPLevel(newXp, newLevel, user.Id, token);
    }

}