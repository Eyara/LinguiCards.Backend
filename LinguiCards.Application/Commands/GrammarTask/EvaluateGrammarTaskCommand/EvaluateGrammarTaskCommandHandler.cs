using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.GrammarTask.EvaluateGrammarTaskCommand;

public class EvaluateGrammarTaskCommandHandler : IRequestHandler<EvaluateGrammarTaskCommand, string>
{
    private readonly IOpenAIService _openAiService;
    private readonly IUsersRepository _usersRepository;
    private readonly IGrammarTaskHistoryRepository _grammarTaskHistoryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUserSettingRepository _userSettingRepository;

    public EvaluateGrammarTaskCommandHandler(
        IOpenAIService openAiService,
        IUsersRepository usersRepository,
        IGrammarTaskHistoryRepository grammarTaskHistoryRepository,
        ILanguageRepository languageRepository,
        IDailyGoalRepository dailyGoalRepository,
        IUserSettingRepository userSettingRepository)
    {
        _openAiService = openAiService;
        _usersRepository = usersRepository;
        _grammarTaskHistoryRepository = grammarTaskHistoryRepository;
        _languageRepository = languageRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<string> Handle(EvaluateGrammarTaskCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (language == null)
        {
            throw new LanguageNotFoundException();
        }

        var response = await _openAiService.GetChatResponseAsync(
            GrammarTaskPrompts.GetEvaluationGrammarTaskPrompt(language.Name, request.Level, request.TaskText,
                request.UserAnswer, request.Topic, request.Type));

        var (accuracy, expectedAnswer, explanation) = ParseGrammarTaskEvaluationOpenAi.ParseEvaluation(response);

        var level = LearningSettings.LanguageLevelToNumber(request.Level);
        await UpdateXpLevel(user, level, accuracy, cancellationToken);

        await _grammarTaskHistoryRepository.AddAsync(new GrammarTaskHistoryDTO
        {
            ExpectedAnswer = expectedAnswer,
            UserAnswer = request.UserAnswer,
            Explanation = explanation,
            Accuracy = accuracy,
            Level = request.Level,
            Type = request.Type ?? "",
            Topic = request.Topic ?? "",
            UserId = user.Id,
            LanguageId = request.LanguageId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        return response;
    }

    private async Task UpdateXpLevel(Domain.Entities.User user, int level, double accuracyPercent, CancellationToken token)
    {
        var requiredXp = CalculatorXP.CalculateXpRequired(user.Level);

        accuracyPercent = Math.Clamp(accuracyPercent, 0, 100);
        var xpGained = LearningSettings.SuccessXpStep * level * (accuracyPercent / 100.0);
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

