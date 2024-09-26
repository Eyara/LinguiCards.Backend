using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.LanguageDictionary.GetAvailableLanguagesQuery;

public class GetAvailableLanguagesQueryHandler : IRequestHandler<GetAvailableLanguagesQuery, List<LanguageDictionaryDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly ILanguageDictionaryRepository _languageDictionaryRepository;
    private readonly IUsersRepository _usersRepository;

    public GetAvailableLanguagesQueryHandler(ILanguageDictionaryRepository languageDictionaryRepository,
        ILanguageRepository languageRepository, IUsersRepository usersRepository)
    {
        _languageDictionaryRepository = languageDictionaryRepository;
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
    }

    public async Task<List<LanguageDictionaryDto>> Handle(GetAvailableLanguagesQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var userLanguages = await _languageRepository.GetAllAsync(user.Id, cancellationToken);
        var allLanguages = await _languageDictionaryRepository.GetAllAsync(cancellationToken);

        return allLanguages
            .Where(ld => userLanguages.All(ul => ul.Name != ld.Name))
            .ToList();
    }
}