using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.User.GetUserInfoQuery;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfo>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;

    public GetUserInfoQueryHandler(IUsersRepository usersRepository, ILanguageRepository languageRepository,
        IWordRepository wordRepository, IDailyGoalRepository dailyGoalRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _dailyGoalRepository = dailyGoalRepository;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var dailyGoal = await _dailyGoalRepository.GetTodayGoalByUserId(user.Id, cancellationToken);

        var info = new UserInfo
        {
            Xp = user.XP,
            XpToNextLevel = CalculatorXP.CalculateXpRequired(user.Level),
            DailyXp = dailyGoal?.GainedXp ?? 0,
            Level = user.Level,
            LanguageStats = new List<LanguageStat>()
        };

        var userLanguages = await _languageRepository.GetAllAsync(user.Id, cancellationToken);

        foreach (var language in userLanguages)
        {
            var words = await _wordRepository.GetAllExtendedAsync(language.Id, cancellationToken);
            var learnedCount = words.Count(w => w.PassiveLearnedPercent > LearningSettings.LearnThreshold);
            var trainingDays = words
                .SelectMany(w => w.Histories)
                .Select(h => h.ChangedOn.Date)
                .GroupBy(d => d.Date)
                .Count();

            info.LanguageStats.Add(new LanguageStat
            {
                LanguageName = language.Name,
                TotalWords = words.Count,
                LearnedWords = learnedCount,
                LearnedPercent = words.Count > 0 ? Math.Round(learnedCount / (double)words.Count * 100, 2) : 0,
                TotalTrainingDays = trainingDays
            });
        }

        return info;
    }
}