using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordChangeHistoryRepository
{
    Task AddAsync(int wordId, bool isCorrectAnswer, int type, double passiveLearned, double activeLearned,
        Guid? trainingId, string? correctAnswer, string? answer, CancellationToken token);

    Task<List<WordChangeHistoryDTO>> GetAllById(Guid trainingId, CancellationToken token);
}