using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.Word.AddWordListCommand;

public class AddWordListCommandHandler : IRequestHandler<AddWordListCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public AddWordListCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<bool> Handle(AddWordListCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (language.UserId != user.Id) throw new EntityOwnershipException();

        var currentWords = await _wordRepository.GetAllAsync(request.LanguageId, cancellationToken);

        var wordsToAdd = request.Words.ToList();

        wordsToAdd.RemoveAll(w => currentWords.Any(cw => cw.Name == w.Name));

        await _wordRepository.AddRangeAsync(wordsToAdd, request.LanguageId, cancellationToken);

        return true;
    }
}