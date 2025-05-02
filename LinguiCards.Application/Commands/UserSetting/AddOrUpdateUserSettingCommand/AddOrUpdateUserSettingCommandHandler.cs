using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.UserSetting.AddOrUpdateUserSettingCommand;

public class AddOrUpdateUserSettingCommandHandler : IRequestHandler<AddOrUpdateUserSettingCommand>
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUsersRepository _usersRepository;

    public AddOrUpdateUserSettingCommandHandler(IUsersRepository usersRepository,
        IUserSettingRepository userSettingRepository)
    {
        _usersRepository = usersRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task Handle(AddOrUpdateUserSettingCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        await _userSettingRepository.AddOrUpdateAsync(user.Id, request.ActiveTrainingSize, request.PassiveTrainingSize,
            cancellationToken);
    }
}