using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordRepository
{
    Task<List<WordDto>> GetUnlearned(int languageId, int percentThreshold, CancellationToken token);
    Task AddAsync(WordDto word, int languageId, CancellationToken token);
    Task UpdateLearnLevel(int wordId, int percent, CancellationToken token);
    Task DeleteAsync(int wordId, CancellationToken token);
}