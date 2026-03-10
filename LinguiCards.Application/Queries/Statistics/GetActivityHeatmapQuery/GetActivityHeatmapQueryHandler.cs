using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetActivityHeatmapQuery;

public class GetActivityHeatmapQueryHandler
    : IRequestHandler<GetActivityHeatmapQuery, ActivityHeatmapResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;

    public GetActivityHeatmapQueryHandler(
        IUsersRepository usersRepository,
        IDailyGoalRepository dailyGoalRepository)
    {
        _usersRepository = usersRepository;
        _dailyGoalRepository = dailyGoalRepository;
    }

    public async Task<ActivityHeatmapResponse> Handle(GetActivityHeatmapQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        var year = request.Year ?? DateTime.UtcNow.Year;
        var goals = await _dailyGoalRepository.GetByYearAsync(user.Id, year, cancellationToken);

        var result = new ActivityHeatmapResponse();

        foreach (var goal in goals)
        {
            result.Days.Add(new HeatmapDay
            {
                Date = goal.Date,
                GainedXp = goal.GainedXp,
                TargetXp = goal.TargetXp,
                IsCompleted = goal.IsCompleted,
                ActivityLevel = ComputeActivityLevel(goal.GainedXp, goal.TargetXp)
            });
        }

        return result;
    }

    private static int ComputeActivityLevel(int gainedXp, int targetXp)
    {
        if (gainedXp == 0) return 0;
        if (targetXp == 0) return gainedXp > 0 ? 4 : 0;

        var ratio = gainedXp / (double)targetXp;
        return ratio switch
        {
            < 0.5 => 1,
            < 1.0 => 2,
            < 1.5 => 3,
            _ => 4
        };
    }
}
