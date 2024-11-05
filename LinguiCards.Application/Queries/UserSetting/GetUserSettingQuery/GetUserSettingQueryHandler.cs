using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.UserSetting.GetUserSettingQuery;

public class GetUserSettingQueryHandler : IRequestHandler<GetUserSettingQuery, UserSettingDto?>
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUsersRepository _usersRepository;

    public GetUserSettingQueryHandler(IUsersRepository usersRepository, IUserSettingRepository userSettingRepository)
    {
        _usersRepository = usersRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<UserSettingDto?> Handle(GetUserSettingQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        return await _userSettingRepository.GetByUserIdAsync(user.Id, cancellationToken);
    }
}