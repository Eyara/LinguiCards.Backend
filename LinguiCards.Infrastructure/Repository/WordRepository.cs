using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Helpers;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class WordRepository : IWordRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public WordRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WordDto> GetByIdAsync(int wordId, CancellationToken token)
    {
        var wordEntity = await _dbContext.Words
            .FirstOrDefaultAsync(l => l.Id == wordId, token);

        if (wordEntity == null) return null;

        return new WordDto
        {
            Id = wordEntity.Id,
            LanguageId = wordEntity.LanguageId,
            Name = wordEntity.Name,
            TranslatedName = wordEntity.TranslatedName,
            PassiveLearnedPercent = wordEntity.PassiveLearnedPercent,
            ActiveLearnedPercent = wordEntity.ActiveLearnedPercent,
            LastUpdated = wordEntity.LastUpdated
        };
    }

    public async Task<WordDto> GetByNameAndLanguageIdAsync(int languageId, string name, CancellationToken token)
    {
        var wordEntity = await _dbContext.Words
            .FirstOrDefaultAsync(l => l.LanguageId == languageId && l.Name == name, token);

        if (wordEntity == null) return null;

        return new WordDto
        {
            Id = wordEntity.Id,
            LanguageId = wordEntity.LanguageId,
            Name = wordEntity.Name,
            TranslatedName = wordEntity.TranslatedName,
            PassiveLearnedPercent = wordEntity.PassiveLearnedPercent,
            ActiveLearnedPercent = wordEntity.ActiveLearnedPercent,
            LastUpdated = wordEntity.LastUpdated
        };
    }

    public async Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.ActiveLearnedPercent)
            .ThenByDescending(w => w.PassiveLearnedPercent)
            .ToListAsync(token);

        return wordEntities
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                PassiveLearnedPercent = w.PassiveLearnedPercent,
                ActiveLearnedPercent = w.ActiveLearnedPercent,
                LastUpdated = w.LastUpdated
            })
            .ToList();
    }

    public async Task<PaginatedResult<WordDto>> GetAllPaginatedAsync(int languageId, int pageNumber, int pageSize,
        string filterQuery = "")
    {
        var wordQuery = _dbContext.Words.Where(w => w.LanguageId == languageId);

        if (!string.IsNullOrWhiteSpace(filterQuery))
            wordQuery = wordQuery.Where(w => w.TranslatedName.Contains(filterQuery));

        return await wordQuery
            .OrderByDescending(w => w.ActiveLearnedPercent)
            .ThenByDescending(w => w.PassiveLearnedPercent)
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                PassiveLearnedPercent = w.PassiveLearnedPercent,
                ActiveLearnedPercent = w.ActiveLearnedPercent,
                LastUpdated = w.LastUpdated
            })
            .ToPaginatedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<WordExtendedDTO>> GetAllExtendedAsync(int languageId, CancellationToken token)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.ActiveLearnedPercent)
            .ThenByDescending(w => w.PassiveLearnedPercent)
            .Include(w => w.Histories)
            .ToListAsync(token);

        return wordEntities
            .Select(w => new WordExtendedDTO
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                PassiveLearnedPercent = w.PassiveLearnedPercent,
                ActiveLearnedPercent = w.ActiveLearnedPercent,
                LastUpdated = w.LastUpdated,
                Histories = w.Histories.Select(h => new WordChangeHistoryDTO
                {
                    Id = h.Id,
                    ChangedOn = h.ChangedOn,
                    IsCorrectAnswer = h.IsCorrectAnswer,
                    PassiveLearned = h.PassiveLearned,
                    VocabularyType = h.VocabularyType,
                    ActiveLearned = h.ActiveLearned
                }).ToList()
            })
            .ToList();
    }


    public async Task<List<WordDto>> GetUnlearned(int languageId, double percentThreshold, VocabularyType type,
        CancellationToken token, int top = 15)
    {
        var wordsQuery = type == VocabularyType.Passive
            ? _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.PassiveLearnedPercent < percentThreshold)
            : _dbContext.Words
                .Where(w => w.LanguageId == languageId && w.ActiveLearnedPercent < percentThreshold);

        var wordEntities = await wordsQuery
            .OrderByDescending(w => Guid.NewGuid())
            .Take(top)
            .ToListAsync(token);

        return wordEntities
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                PassiveLearnedPercent = w.PassiveLearnedPercent,
                ActiveLearnedPercent = w.ActiveLearnedPercent,
                LastUpdated = w.LastUpdated
            })
            .ToList();
    }

    public async Task AddAsync(WordDto word, int languageId, CancellationToken token)
    {
        await _dbContext.Words.AddAsync(
            new Word
            {
                Name = word.Name.ToLower(),
                TranslatedName = word.TranslatedName.ToLower(),
                LanguageId = languageId,
                PassiveLearnedPercent = 0,
                ActiveLearnedPercent = 0,
                LastUpdated = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            }, token);

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task AddRangeAsync(IEnumerable<WordDto> words, int languageId, CancellationToken token)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(token);
        try
        {
            var wordEntities = words.Select(word => new Word
            {
                Name = word.Name.ToLower(),
                TranslatedName = word.TranslatedName.ToLower(),
                LanguageId = languageId,
                PassiveLearnedPercent = 0,
                ActiveLearnedPercent = 0,
                LastUpdated = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            }).ToList();

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

        word.Name = name.ToLower();
        word.TranslatedName = translationName.ToLower();

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdatePassiveLearnLevel(int wordId, double passivePercent, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        word.PassiveLearnedPercent = passivePercent;
        word.LastUpdated = DateTime.UtcNow;
        ;
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateActiveLearnLevel(int wordId, double activePercent, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        word.ActiveLearnedPercent = activePercent;
        word.LastUpdated = DateTime.UtcNow;
        ;
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateLearnedPercentRangeAsync(
        List<(int wordId, double passivePercent, double activePercent)> wordUpdates, CancellationToken token)
    {
        var wordIds = wordUpdates.Select(w => w.wordId).ToList();

        var words = await _dbContext.Words
            .Where(w => wordIds.Contains(w.Id))
            .ToListAsync(token);

        if (!words.Any()) throw new Exception("No words found.");

        foreach (var word in words)
        {
            var updateInfo = wordUpdates.First(w => w.wordId == word.Id);
            word.PassiveLearnedPercent = updateInfo.passivePercent;
            word.ActiveLearnedPercent = updateInfo.activePercent;
            word.LastUpdated = DateTime.UtcNow;
        }

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