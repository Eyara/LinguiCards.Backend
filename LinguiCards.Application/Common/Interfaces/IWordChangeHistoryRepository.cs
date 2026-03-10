using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordChangeHistoryRepository
{
    Task AddAsync(int wordId, bool isCorrectAnswer, int type, double passiveLearned, double activeLearned,
        Guid? trainingId, string? correctAnswer, string? answer, CancellationToken token);

    Task<List<WordChangeHistoryDTO>> GetAllByIdAsync(Guid trainingId, CancellationToken token);

    Task<List<WordChangeHistoryDTO>> GetByLanguageIdAsync(int languageId, DateTime? from,
        CancellationToken token);

    Task<Dictionary<int, List<WordChangeHistoryDTO>>> GetGroupedByWordAsync(int languageId, int? minAttempts,
        CancellationToken token);
}