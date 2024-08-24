using LinguiCards.Application.Common;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.User.AddUserCommand;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, RequestResult>
{
    private readonly IUsersRepository _usersRepository;

    public AddUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<RequestResult> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (currentUser != null) return new RequestResult { Result = false, Error = "The user is already exists" };

        PasswordHasher.CreatePasswordHash(request.Password, out var passwordHash, out var salt);

        await _usersRepository.AddAsync(
            new Domain.Entities.User { Username = request.Username, PasswordHash = passwordHash, Salt = salt },
            cancellationToken);

        return new RequestResult { Result = true };
    }
}