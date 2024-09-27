using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.Word.AddWordCommand;

public class AddWordCommandHandler : IRequestHandler<AddWordCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public AddWordCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<bool> Handle(AddWordCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (language.UserId != user.Id) throw new EntityOwnershipException();

        var wordEntity =
            await _wordRepository.GetByNameAndLanguageIdAsync(request.LanguageId, request.Word.Name, cancellationToken);

        if (wordEntity != null)
        {
            throw new WordAlreadyExistsException();
        }

        await _wordRepository.AddAsync(request.Word, request.LanguageId, cancellationToken);

        return true;
    }
}