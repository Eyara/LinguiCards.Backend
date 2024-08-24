using LinguiCards.Application.Common.Interfaces;
using MediatR;

namespace LinguiCards.Application.Commands.Language.DeleteLanguageCommand;

public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;

    public DeleteLanguageCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new Exception();

        var languageEntity = await _languageRepository.GetByIdAsync(request.Id, cancellationToken);

        if (languageEntity.UserId != user.Id) throw new Exception();

        await _languageRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}