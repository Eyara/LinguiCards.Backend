using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.UserSetting.AddOrUpdateUserSettingCommand;

public class AddOrUpdateUserSettingCommandHandler : IRequestHandler<AddOrUpdateUserSettingCommand>
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IDailyGoalRepository _dailyGoalRepository;

    public AddOrUpdateUserSettingCommandHandler(IUsersRepository usersRepository,
        IUserSettingRepository userSettingRepository,
        IDailyGoalRepository dailyGoalRepository)
    {
        _usersRepository = usersRepository;
        _userSettingRepository = userSettingRepository;
        _dailyGoalRepository = dailyGoalRepository;
    }

    public async Task Handle(AddOrUpdateUserSettingCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        await _userSettingRepository.AddOrUpdateAsync(user.Id, request.ActiveTrainingSize, request.PassiveTrainingSize, request.DailyGoalXp,
            request.DailyGoalByTranslation, request.DailyGoalByGrammar, cancellationToken);

        var dailyGoal = await _dailyGoalRepository.GetTodayGoalByUserId(user.Id, cancellationToken);

        await _dailyGoalRepository.SetTotalXpAsync(user.Id, dailyGoal?.GainedXp ?? 0, request.DailyGoalXp ?? 0, cancellationToken);
    }
}