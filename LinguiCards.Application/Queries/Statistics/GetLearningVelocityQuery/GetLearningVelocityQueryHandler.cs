using System.Globalization;
using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Statistics;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetLearningVelocityQuery;

public class GetLearningVelocityQueryHandler
    : IRequestHandler<GetLearningVelocityQuery, LearningVelocityResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;
    private readonly IWordChangeHistoryRepository _historyRepository;

    public GetLearningVelocityQueryHandler(
        IUsersRepository usersRepository,
        IWordRepository wordRepository,
        IWordChangeHistoryRepository historyRepository)
    {
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _historyRepository = historyRepository;
    }

    public async Task<LearningVelocityResponse> Handle(GetLearningVelocityQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        var rangeStart = StartOfWeek(DateTime.UtcNow).AddDays(-7 * (request.Weeks - 1));

        var words = await _wordRepository.GetAllAsync(request.LanguageId, cancellationToken);
        var histories = await _historyRepository.GetByLanguageIdAsync(
            request.LanguageId, rangeStart, cancellationToken);

        var wordsAddedByWeek = words
            .Where(w => w.CreatedOn >= rangeStart)
            .GroupBy(w => StartOfWeek(w.CreatedOn))
            .ToDictionary(g => g.Key, g => g.Count());

        var learnedByWeek = new Dictionary<DateTime, int>();
        var threshold = LearningSettings.LearnThreshold;

        var historiesByWord = histories
            .GroupBy(h => h.WordId)
            .ToDictionary(g => g.Key, g => g.OrderBy(h => h.ChangedOn).ToList());

        foreach (var (wordId, wordHistories) in historiesByWord)
        {
            var learnedEntry = wordHistories.FirstOrDefault(h =>
                h.PassiveLearned >= threshold && h.ActiveLearned >= threshold);

            if (learnedEntry != null)
            {
                var week = StartOfWeek(learnedEntry.ChangedOn);
                learnedByWeek.TryGetValue(week, out var count);
                learnedByWeek[week] = count + 1;
            }
        }

        var result = new LearningVelocityResponse();
        for (var i = 0; i < request.Weeks; i++)
        {
            var weekStart = rangeStart.AddDays(7 * i);
            wordsAddedByWeek.TryGetValue(weekStart, out var added);
            learnedByWeek.TryGetValue(weekStart, out var learned);

            result.Weeks.Add(new WeeklyVelocity
            {
                WeekStart = weekStart,
                WordsAdded = added,
                WordsLearned = learned
            });
        }

        return result;
    }

    private static DateTime StartOfWeek(DateTime dt)
    {
        var diff = (7 + (dt.Date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.Date.AddDays(-diff);
    }
}
