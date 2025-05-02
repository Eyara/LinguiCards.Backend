using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordRepository
{
    Task<WordDto> GetByIdAsync(int wordId, CancellationToken token);
    Task<WordDto> GetByNameAndLanguageIdAsync(int languageId, string name, CancellationToken token);
    Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token);

    Task<PaginatedResult<WordDto>> GetAllPaginatedAsync(int languageId, int pageNumber, int pageSize,
        string nameFilterQuery = "", string translationNameFilterQuery = "");

    Task<List<WordExtendedDTO>> GetAllExtendedAsync(int languageId, CancellationToken token);

    Task<List<WordDto>> GetUnlearned(int languageId, double percentThreshold, VocabularyType type,
        CancellationToken token, int? top = 15);

    Task AddAsync(WordDto word, int languageId, CancellationToken token);
    Task AddRangeAsync(IEnumerable<WordDto> words, int languageId, CancellationToken token);
    Task UpdateAsync(int wordId, string name, string translationName, CancellationToken token);
    Task UpdatePassiveLearnLevel(int wordId, double passivePercent, CancellationToken token);
    Task UpdateActiveLearnLevel(int wordId, double activePercent, CancellationToken token);

    Task UpdateLearnedPercentRangeAsync(List<(int wordId, double passivePercent, double activePercent)> wordUpdates,
        CancellationToken token);

    Task DeleteAsync(int wordId, CancellationToken token);
}