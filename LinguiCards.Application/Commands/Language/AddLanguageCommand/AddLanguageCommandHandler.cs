﻿using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.Language.AddLanguageCommand;

public class AddLanguageCommandHandler : IRequestHandler<AddLanguageCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;

    public AddLanguageCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(AddLanguageCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language =
            await _languageRepository.GetByNameAndUserAsync(request.Language.Name, user.Id, cancellationToken);

        if (language != null) throw new LanguageAlreadyExistsException();

        await _languageRepository.AddAsync(request.Language, user.Id, cancellationToken);
        return true;
    }
}