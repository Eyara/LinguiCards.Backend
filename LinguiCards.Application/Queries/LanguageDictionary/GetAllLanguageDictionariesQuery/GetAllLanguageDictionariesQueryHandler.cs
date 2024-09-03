using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.LanguageDictionary.GetAllLanguageDictionariesQuery;

public class
    GetAllLanguageDictionariesQueryHandler : IRequestHandler<GetAllLanguageDictionariesQuery,
        List<LanguageDictionaryDto>>
{
    private readonly ILanguageDictionaryRepository _languageDictionaryRepository;

    public GetAllLanguageDictionariesQueryHandler(ILanguageDictionaryRepository languageDictionaryRepository)
    {
        _languageDictionaryRepository = languageDictionaryRepository;
    }

    public async Task<List<LanguageDictionaryDto>> Handle(GetAllLanguageDictionariesQuery request,
        CancellationToken cancellationToken)
    {
        return await _languageDictionaryRepository.GetAllAsync(cancellationToken);
    }
}