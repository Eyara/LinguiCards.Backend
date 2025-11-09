using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IGrammarTaskHistoryRepository
{
    Task AddAsync(GrammarTaskHistoryDTO historyRecord, CancellationToken token);
}

