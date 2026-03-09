using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IWordRepository
{
    Task<WordDto> GetByIdAsync(int wordId, CancellationToken token);
    Task<WordExtendedDTO> GetByIdExtendedAsync(int wordId, CancellationToken token);
    Task<WordDto> GetByNameAndLanguageIdAsync(int languageId, string name, CancellationToken token);
    Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token);

    Task<PaginatedResult<WordDto>> GetAllPaginatedAsync(int languageId, int pageNumber, int pageSize,
        string nameFilterQuery = "", string translationNameFilterQuery = "");

    Task<List<WordExtendedDTO>> GetAllExtendedAsync(int languageId, CancellationToken token);

    Task<List<WordDto>> GetDueForReview(int languageId, VocabularyType type, int limit, CancellationToken token);

    Task<int> CountDueForReview(int languageId, VocabularyType type, DateTime asOf, CancellationToken token);
    Task<int> CountNewWords(int languageId, VocabularyType type, CancellationToken token);

    Task AddAsync(WordDto word, int languageId, CancellationToken token);
    Task AddRangeAsync(IEnumerable<WordDto> words, int languageId, CancellationToken token);
    Task UpdateAsync(int wordId, string name, string translationName, CancellationToken token);

    Task UpdateSrsState(int wordId, VocabularyType vocabularyType, double easeFactor, int intervalDays,
        int repetitionCount, DateTime nextReviewDate, double learnedPercent, CancellationToken token);

    Task DeleteAsync(int wordId, CancellationToken token);
}