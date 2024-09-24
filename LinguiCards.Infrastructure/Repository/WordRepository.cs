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
            LearnedPercent = wordEntity.LearnedPercent,
            LastUpdated = wordEntity.LastUpdated
        };
    }

    public async Task<List<WordDto>> GetAllAsync(int languageId, CancellationToken token)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.LearnedPercent)
            .ToListAsync(token);

        return wordEntities
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                LearnedPercent = w.LearnedPercent,
                LastUpdated = w.LastUpdated
            })
            .ToList();
    }

    public async Task<PaginatedResult<WordDto>> GetAllPaginatedAsync(int languageId, int pageNumber, int pageSize)
    {
        return await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.LearnedPercent)
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                LearnedPercent = w.LearnedPercent,
                LastUpdated = w.LastUpdated
            })
            .ToPaginatedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<WordExtendedDTO>> GetAllExtendedAsync(int languageId, CancellationToken token)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId)
            .OrderByDescending(w => w.LearnedPercent)
            .Include(w => w.Histories)
            .ToListAsync(token);

        return wordEntities
            .Select(w => new WordExtendedDTO
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                Name = w.Name,
                TranslatedName = w.TranslatedName,
                LearnedPercent = w.LearnedPercent,
                LastUpdated = w.LastUpdated,
                Histories = w.Histories.Select(h => new WordChangeHistoryDTO
                {
                    Id = h.Id,
                    ChangedOn = h.ChangedOn,
                    IsCorrectAnswer = h.IsCorrectAnswer
                }).ToList()
            })
            .ToList();
    }


    public async Task<List<WordDto>> GetUnlearned(int languageId, double percentThreshold, CancellationToken token, int top = 15)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId && w.LearnedPercent < percentThreshold)
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
                LearnedPercent = w.LearnedPercent,
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
                LearnedPercent = 0,
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
                LearnedPercent = 0,
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

    public async Task UpdateLearnLevel(int wordId, double percent, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null) throw new Exception();

        word.LearnedPercent = percent;
        word.LastUpdated = DateTime.UtcNow;
        ;
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