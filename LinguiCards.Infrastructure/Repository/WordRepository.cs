using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
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

    public async Task<List<WordDto>> GetUnlearned(int languageId, int percentThreshold, CancellationToken token)
    {
        var wordEntities = await _dbContext.Words
            .Where(w => w.LanguageId == languageId && w.LearnedPercent < percentThreshold)
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
                Name = word.Name, TranslatedName = word.TranslatedName, LanguageId = languageId, LearnedPercent = 0
            }, token);

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateLearnLevel(int wordId, int percent, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null)
        {
            throw new Exception();
        }

        word.LearnedPercent = percent;
        word.LastUpdated = DateTime.Now;;
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int wordId, CancellationToken token)
    {
        var word = await _dbContext.Words
            .FirstOrDefaultAsync(w => w.Id == wordId, token);

        if (word == null)
        {
            throw new Exception();
        }

        _dbContext.Remove(word);
        await _dbContext.SaveChangesAsync(token);
    }
}