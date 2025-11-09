using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.GrammarTaskType.GetAllGrammarTaskTypeDictionariesQuery;

public class GetAllGrammarTaskTypeDictionariesQueryHandler : IRequestHandler<GetAllGrammarTaskTypeDictionariesQuery, List<GrammarTaskTypeDictionaryDto>>
{
    private readonly IGrammarTaskTypeDictionaryRepository _grammarTaskTypeDictionaryRepository;

    public GetAllGrammarTaskTypeDictionariesQueryHandler(IGrammarTaskTypeDictionaryRepository grammarTaskTypeDictionaryRepository)
    {
        _grammarTaskTypeDictionaryRepository = grammarTaskTypeDictionaryRepository;
    }

    public async Task<List<GrammarTaskTypeDictionaryDto>> Handle(GetAllGrammarTaskTypeDictionariesQuery request,
        CancellationToken cancellationToken)
    {
        return await _grammarTaskTypeDictionaryRepository.GetAllAsync(cancellationToken);
    }
}

