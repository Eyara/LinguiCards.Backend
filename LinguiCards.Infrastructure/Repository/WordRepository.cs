using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Helpers;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class WordRepository : IWordRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public WordRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WordDto> GetByIdAsync(int wordId, CancellationToken token)
    {
        var wordEntity = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (wordEntity == null) return null;

        return _mapper.Map<WordDto>(wordEntity);
    }

    public async Task<WordExtendedDTO> GetByIdExtendedAsync(int wordId, CancellationToken token)
    {
        var wordEntity = await _dbContext.Words
            .Include(w => w.Histories)
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (wordEntity == null) return null;

        return _mapper.Map<WordExtendedDTO>(wordEntity);
    }

    public async Task<WordDto> GetByNameAndLanguageIdAsync(int languageId, string name, CancellationToken token)
    {
        var wordEntity = await _dbContext.Words
            .FirstOrDefaultAsync(l => l.LanguageId == languageId && l.Name == name, token);

        if (wordEntity == null) return null;

        return _mapper.Map<WordDto>(wordEntity);
    }

    public async Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token)
    {
        return await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.ActiveLearnedPercent)
            .ThenByDescending(w => w.PassiveLearnedPercent)
            .ProjectTo<WordDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }

    public async Task<List<WordDto>> GetDueForReview(int languageId, VocabularyType type, int limit,
        CancellationToken token)
    {
        var now = DateTime.UtcNow;

        var overdueQuery = type == VocabularyType.Passive
            ? _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.PassiveNextReviewDate != null &&
                            w.PassiveNextReviewDate <= now)
                .OrderBy(w => w.PassiveNextReviewDate)
            : _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.ActiveNextReviewDate != null &&
                            w.ActiveNextReviewDate <= now)
                .OrderBy(w => w.ActiveNextReviewDate);

        var overdueWords = await overdueQuery
            .Take(limit)
            .ProjectTo<WordDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);

        if (overdueWords.Count >= limit)
            return overdueWords;

        var remaining = limit - overdueWords.Count;
        var overdueIds = overdueWords.Select(w => w.Id).ToHashSet();

        var newWordsQuery = type == VocabularyType.Passive
            ? _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.PassiveNextReviewDate == null &&
                            !overdueIds.Contains(w.Id))
                .OrderBy(w => w.CreatedOn)
            : _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.ActiveNextReviewDate == null &&
                            !overdueIds.Contains(w.Id))
                .OrderBy(w => w.CreatedOn);

        var newWords = await newWordsQuery
            .Take(remaining)
            .ProjectTo<WordDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);

        overdueWords.AddRange(newWords);
        return overdueWords;
    }

    public async Task<int> CountDueForReview(int languageId, VocabularyType type, DateTime asOf,
        CancellationToken token)
    {
        if (type == VocabularyType.Passive)
            return await _dbContext.Words
                .CountAsync(
                    w => w.LanguageId == languageId && w.PassiveNextReviewDate != null &&
                         w.PassiveNextReviewDate <= asOf, token);

        return await _dbContext.Words
            .CountAsync(
                w => w.LanguageId == languageId && w.ActiveNextReviewDate != null &&
                     w.ActiveNextReviewDate <= asOf, token);
    }

    public async Task<int> CountNewWords(int languageId, VocabularyType type, CancellationToken token)
    {
        if (type == VocabularyType.Passive)
            return await _dbContext.Words
                .CountAsync(w => w.LanguageId == languageId && w.PassiveNextReviewDate == null, token);

        return await _dbContext.Words
            .CountAsync(w => w.LanguageId == languageId && w.ActiveNextReviewDate == null, token);
    }

    public async Task<PaginatedResult<WordDto>> GetAllPaginatedAsync(int languageId, int pageNumber, int pageSize,
        string nameFilterQuery = "", string translationNameFilterQuery = "")
    {
        var wordQuery = _dbContext.Words.Where(w => w.LanguageId == languageId);

        if (!string.IsNullOrWhiteSpace(nameFilterQuery))
            wordQuery = wordQuery.Where(w => w.Name.ToLower().Trim().Contains(nameFilterQuery.ToLower().Trim()));

        if (!string.IsNullOrWhiteSpace(translationNameFilterQuery))
            wordQuery = wordQuery.Where(w =>
                w.TranslatedName.ToLower().Trim().Contains(translationNameFilterQuery.ToLower().Trim()));

        var wordDtoQuery = wordQuery
            .ProjectTo<WordDto>(_mapper.ConfigurationProvider);

        // Check if pagination should be skipped
        if (!string.IsNullOrWhiteSpace(nameFilterQuery) || !string.IsNullOrWhiteSpace(translationNameFilterQuery))
        {
            var results = await wordDtoQuery.ToListAsync();
            return new PaginatedResult<WordDto>(results, results.Count, 1, results.Count);
        }

        // Apply pagination if no query filters
        return await wordDtoQuery.ToPaginatedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<WordExtendedDTO>> GetAllExtendedAsync(int languageId, CancellationToken token)
    {
        return await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.ActiveLearnedPercent)
            .ThenByDescending(w => w.PassiveLearnedPercent)
            .Include(w => w.Histories)
            .ProjectTo<WordExtendedDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }



    public async Task AddAsync(WordDto word, int languageId, CancellationToken token)
    {
        var entity = _mapper.Map<Word>(word);
        entity.LanguageId = languageId;
        await _dbContext.Words.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task AddRangeAsync(IEnumerable<WordDto> words, int languageId, CancellationToken token)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(token);
        try
        {
            var wordEntities = _mapper.Map<List<Word>>(words);

            foreach (var word in wordEntities) word.LanguageId = languageId; // Set externally passed value

            await _dbContext.AddRangeAsync(wordEntities, token);
            await _dbContext.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            throw new Exception("An error occurred while adding the word range", ex);
        }
    }

    public async Task UpdateAsync(int wordId, string name, string translationName, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        word.Name = name.ToLower().Trim().Replace('ё', 'е');
        word.TranslatedName = translationName.ToLower().Trim().Replace('ё', 'е');

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateSrsState(int wordId, VocabularyType vocabularyType, double easeFactor,
        int intervalDays, int repetitionCount, DateTime nextReviewDate, double learnedPercent,
        CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        if (vocabularyType == VocabularyType.Active)
        {
            word.ActiveEaseFactor = easeFactor;
            word.ActiveIntervalDays = intervalDays;
            word.ActiveRepetitionCount = repetitionCount;
            word.ActiveNextReviewDate = nextReviewDate;
            word.ActiveLearnedPercent = learnedPercent;
        }
        else
        {
            word.PassiveEaseFactor = easeFactor;
            word.PassiveIntervalDays = intervalDays;
            word.PassiveRepetitionCount = repetitionCount;
            word.PassiveNextReviewDate = nextReviewDate;
            word.PassiveLearnedPercent = learnedPercent;
        }

        word.LastUpdated = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(token);
    }


    public async Task DeleteAsync(int wordId, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        _dbContext.Remove(word);
        await _dbContext.SaveChangesAsync(token);
    }
}