using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.DailyGoal.AddOrUpdateDailyGoalCommand;

public class AddOrUpdateDailyGoalCommandHandler : IRequestHandler<AddOrUpdateDailyGoalCommand>
{
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IUserSettingRepository _userSettingRepository;

    public AddOrUpdateDailyGoalCommandHandler(IUsersRepository usersRepository,
        IDailyGoalRepository dailyGoalRepository,
        IUserSettingRepository userSettingRepository)
    {
        _usersRepository = usersRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task Handle(AddOrUpdateDailyGoalCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var userSettings = await _userSettingRepository.GetByUserIdAsync(user.Id, cancellationToken);

        if (userSettings == null) throw new NotFoundException("Настройки пользователя не найдены");

        await _dailyGoalRepository.AddOrUpdateAsync(user.Id, request.GainedXp, userSettings.DailyGoalXp ?? 0,
            cancellationToken);
    }
}