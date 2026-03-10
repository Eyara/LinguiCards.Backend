using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface ITranslationEvaluationHistoryRepository
{
    Task AddAsync(TranslationEvaluationHistoryDTO historyRecord, CancellationToken token);

    Task<List<TranslationEvaluationHistoryDTO>> GetByLanguageIdAsync(int userId, int? languageId,
        CancellationToken token);
}