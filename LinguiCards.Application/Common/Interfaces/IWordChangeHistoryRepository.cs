namespace LinguiCards.Application.Common.Interfaces;

public interface IWordChangeHistoryRepository
{
    Task AddAsync(int wordId, bool isCorrectAnswer, CancellationToken token);
}