using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.DailyGoal.GetTodayGoalQuery;

public class GetTodayGoalQueryHandler : IRequestHandler<GetTodayGoalQuery, DailyGoalDTO?>
{
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IUserSettingRepository _userSettingRepository;

    public GetTodayGoalQueryHandler(IUsersRepository usersRepository,
        IDailyGoalRepository dailyGoalRepository,
        IUserSettingRepository userSettingRepository)
    {
        _usersRepository = usersRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<DailyGoalDTO?> Handle(GetTodayGoalQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        return await _dailyGoalRepository.GetTodayGoalByUserId(user.Id, cancellationToken);
    }
}