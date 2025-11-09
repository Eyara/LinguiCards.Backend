using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IGrammarTaskTypeDictionaryRepository
{
    Task<List<GrammarTaskTypeDictionaryDto>> GetAllAsync(CancellationToken token);
}

