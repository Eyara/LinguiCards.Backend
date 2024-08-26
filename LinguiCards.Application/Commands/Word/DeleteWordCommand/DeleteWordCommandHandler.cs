using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.Word.DeleteWordCommand;

public class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public DeleteWordCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<bool> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new Exception();

        var wordEntity = await _wordRepository.GetByIdAsync(request.Id, cancellationToken);

        if (wordEntity == null) throw new Exception();

        var languageEntity = await _languageRepository.GetByIdAsync(wordEntity.LanguageId, cancellationToken);

        if (languageEntity == null) throw new Exception();

        if (languageEntity.UserId != user.Id) throw new Exception();

        await _wordRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}