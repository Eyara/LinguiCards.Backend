using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetRetentionCurveQuery;

public class GetRetentionCurveQueryHandler
    : IRequestHandler<GetRetentionCurveQuery, RetentionCurveResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IWordChangeHistoryRepository _historyRepository;

    public GetRetentionCurveQueryHandler(
        IUsersRepository usersRepository,
        IWordChangeHistoryRepository historyRepository)
    {
        _usersRepository = usersRepository;
        _historyRepository = historyRepository;
    }

    public async Task<RetentionCurveResponse> Handle(GetRetentionCurveQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        var from = DateTime.UtcNow.Date.AddDays(-request.RangeDays);
        var histories = await _historyRepository.GetByLanguageIdAsync(
            request.LanguageId, from, cancellationToken);

        var result = new RetentionCurveResponse();
        var periodStart = from;

        while (periodStart < DateTime.UtcNow.Date)
        {
            var periodEnd = periodStart.AddDays(request.PeriodDays);
            var bucket = histories
                .Where(h => h.ChangedOn >= periodStart && h.ChangedOn < periodEnd)
                .ToList();

            if (bucket.Count > 0)
            {
                var correct = bucket.Count(h => h.IsCorrectAnswer);
                result.Periods.Add(new RetentionPeriod
                {
                    PeriodStart = periodStart,
                    TotalAnswers = bucket.Count,
                    CorrectAnswers = correct,
                    Accuracy = Math.Round(correct / (double)bucket.Count * 100, 2)
                });
            }
            else
            {
                result.Periods.Add(new RetentionPeriod
                {
                    PeriodStart = periodStart
                });
            }

            periodStart = periodEnd;
        }

        return result;
    }
}
