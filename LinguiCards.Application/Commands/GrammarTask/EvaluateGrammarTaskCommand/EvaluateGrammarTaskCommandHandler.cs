using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Common.Models.Integration;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.GrammarTask.EvaluateGrammarTaskCommand;

public class EvaluateGrammarTaskCommandHandler : IRequestHandler<EvaluateGrammarTaskCommand, GrammarTaskEvaluationDTO>
{
    private readonly IOpenAIService _openAiService;
    private readonly IUsersRepository _usersRepository;
    private readonly IGrammarTaskHistoryRepository _grammarTaskHistoryRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly ILanguageRepository _languageRepository;

    public EvaluateGrammarTaskCommandHandler(
        IOpenAIService openAiService,
        IUsersRepository usersRepository,
        IGrammarTaskHistoryRepository grammarTaskHistoryRepository,
        IDailyGoalRepository dailyGoalRepository,
        ILanguageRepository languageRepository)
    {
        _openAiService = openAiService;
        _usersRepository = usersRepository;
        _grammarTaskHistoryRepository = grammarTaskHistoryRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _languageRepository = languageRepository;
    }

    public async Task<GrammarTaskEvaluationDTO> Handle(EvaluateGrammarTaskCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        // Get language name from user's languages
        var userLanguages = await _languageRepository.GetAllAsync(user.Id, cancellationToken);
        var languageName = userLanguages.FirstOrDefault()?.Name ?? "Латынь";

        var response = await _openAiService.GetChatResponseAsync(
            GrammarTaskPrompts.GetEvaluationGrammarTaskPrompt(languageName, request.Level, request.TaskText,
                request.UserAnswer, request.Topic, request.Type));

        var parsedEvaluation = ParseGrammarTaskEvaluationOpenAI.ParseEvaluation(response);

        await UpdateXpLevel(user, LearningSettings.LanguageLevelToNumber(request.Level), 1,
            parsedEvaluation.Accuracy, cancellationToken);

        await _grammarTaskHistoryRepository.AddAsync(new GrammarTaskHistoryDTO
        {
            ExpectedAnswer = parsedEvaluation.ExpectedAnswer,
            UserAnswer = request.UserAnswer,
            Explanation = parsedEvaluation.Explanation,
            Level = request.Level,
            Type = request.Type ?? "",
            Topic = request.Topic ?? "",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        return parsedEvaluation;
    }

    private async Task UpdateXpLevel(Domain.Entities.User user, int level, int taskCount, double accuracyPercent,
        CancellationToken token)
    {
        var requiredXp = CalculatorXP.CalculateXpRequired(user.Level);

        accuracyPercent = Math.Clamp(accuracyPercent, 0, 100);
        var xpGained = LearningSettings.SuccessXpStep * level * taskCount * (accuracyPercent / 100.0);
        var newXp = xpGained + user.XP;

        var newLevel = user.Level;

        if (newXp >= requiredXp)
        {
            newLevel++;
            newXp -= requiredXp;
        }

        await _usersRepository.UpdateXPLevel(newXp, newLevel, user.Id, token);
        
        // Update daily goal with XP delta
        await _dailyGoalRepository.AddXpAsync(user.Id, (int)xpGained, token);
    }
}

