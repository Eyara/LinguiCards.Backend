using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.DailyGoal.GetDailyGoalStatusQuery;

public class GetDailyGoalStatusQueryHandler : IRequestHandler<GetDailyGoalStatusQuery, DailyGoalStatusDto>
{
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUsersRepository _usersRepository;

    public GetDailyGoalStatusQueryHandler(IUsersRepository usersRepository,
        IDailyGoalRepository dailyGoalRepository)
    {
        _usersRepository = usersRepository;
        _dailyGoalRepository = dailyGoalRepository;
    }

    public async Task<DailyGoalStatusDto> Handle(GetDailyGoalStatusQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var dailyGoal = await _dailyGoalRepository.GetTodayGoalByUserId(user.Id, cancellationToken);

        if (dailyGoal == null)
        {
            return new DailyGoalStatusDto
            {
                Username = request.Username,
                IsGoalCompleted = false,
                GainedXp = 0,
                TargetXp = 0
            };
        }

        return new DailyGoalStatusDto
        {
            Username = request.Username,
            IsGoalCompleted = dailyGoal.IsCompleted,
            GainedXp = dailyGoal.GainedXp,
            TargetXp = dailyGoal.TargetXp
        };
    }
}
