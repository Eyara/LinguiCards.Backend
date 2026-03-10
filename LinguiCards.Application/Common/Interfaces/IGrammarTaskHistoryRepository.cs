using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IGrammarTaskHistoryRepository
{
    Task AddAsync(GrammarTaskHistoryDTO historyRecord, CancellationToken token);

    Task<List<GrammarTaskHistoryDTO>> GetByLanguageIdAsync(int userId, int? languageId, DateTime? from,
        CancellationToken token);
}

