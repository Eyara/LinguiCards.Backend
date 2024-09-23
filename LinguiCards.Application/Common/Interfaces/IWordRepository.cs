using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordRepository
{
    Task<WordDto> GetByIdAsync(int wordId, CancellationToken token);
    Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token);
    Task<List<WordDto>> GetUnlearned(int languageId, double percentThreshold, CancellationToken token, int top = 15);
    Task AddAsync(WordDto word, int languageId, CancellationToken token);
    Task AddRangeAsync(IEnumerable<WordDto> words, int languageId, CancellationToken token);
    Task UpdateAsync(int wordId, string name, string translationName, CancellationToken token);
    Task UpdateLearnLevel(int wordId, double percent, CancellationToken token);
    Task DeleteAsync(int wordId, CancellationToken token);
}