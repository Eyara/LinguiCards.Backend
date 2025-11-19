using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Common.Models.Integration;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.TranslationEvaluation.EvaluateTranslationCommand;

public class
    EvaluateTranslationCommandHandler : IRequestHandler<EvaluateTranslationCommand, TranslationEvaluationDTO>
{
    private readonly IOpenAIService _openAiService;
    private readonly IUsersRepository _usersRepository;
    private readonly ITranslationEvaluationHistoryRepository _evaluationHistoryRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUserSettingRepository _userSettingRepository;

    public EvaluateTranslationCommandHandler(IOpenAIService openAiService, IUsersRepository usersRepository,
        ITranslationEvaluationHistoryRepository translationEvaluationHistoryRepository,
        IDailyGoalRepository dailyGoalRepository, IUserSettingRepository userSettingRepository)
    {
        _openAiService = openAiService;
        _usersRepository = usersRepository;
        _evaluationHistoryRepository = translationEvaluationHistoryRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<TranslationEvaluationDTO> Handle(EvaluateTranslationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var response = await _openAiService.GetChatResponseAsync(
            TranslationPrompts.GetEvaluationTranslationPrompt("Латынь", request.Level, request.OriginalText,
                request.Translation));

        var parsedEvaluation = ParseEvaluationOpenAI.ParseEvaluation(response);
        var wordList = request.OriginalText.Split(' ').ToList();

        await UpdateXpLevel(user, LearningSettings.LanguageLevelToNumber(request.Level), wordList.Count,
            parsedEvaluation.Accuracy, cancellationToken);

        await _evaluationHistoryRepository.AddAsync(new TranslationEvaluationHistoryDTO
        {
            OriginalText = request.OriginalText,
            UserTranslation = request.Translation,
            CorrectTranslation = parsedEvaluation.CorrectTranslation,
            Accuracy = parsedEvaluation.Accuracy,
            Level = request.Level,
            MinorIssues = string.Join(";;", parsedEvaluation.MinorIssues),
            Errors = string.Join(";;", parsedEvaluation.Errors),
            CriticalErrors = string.Join(";;", parsedEvaluation.CriticalErrors),
            UserId = user.Id,
            LanguageId = request.LanguageId
        }, cancellationToken);

        return parsedEvaluation;
    }

    private async Task UpdateXpLevel(Domain.Entities.User user, int level, int wordCount, double accuracyPercent,
        CancellationToken token)
    {
        var requiredXp = CalculatorXP.CalculateXpRequired(user.Level);

        accuracyPercent = Math.Clamp(accuracyPercent, 0, 100);
        var xpGained = LearningSettings.SuccessXpStep * level * wordCount * (accuracyPercent / 100.0);
        var newXp = xpGained + user.XP;

        var newLevel = user.Level;

        if (newXp >= requiredXp)
        {
            newLevel++;
            newXp -= requiredXp;
        }

        await _usersRepository.UpdateXPLevel(newXp, newLevel, user.Id, token);
        
        var userSettings = await _userSettingRepository.GetByUserIdAsync(user.Id, token);
        var targetXp = userSettings?.DailyGoalXp ?? 0;
        
        await _dailyGoalRepository.AddXpAsync(user.Id, (int)xpGained, targetXp, token);
    }
}