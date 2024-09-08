using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Language.GetAllLanguagesQuery;

public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, List<LanguageDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;

    public GetAllLanguagesQueryHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
    }

    public async Task<List<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        return await _languageRepository.GetAllAsync(user.Id, cancellationToken);
    }
}