using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class LanguageRepository : ILanguageRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public LanguageRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LanguageDto>> GetAllAsync(int userId, CancellationToken token)
    {
        var languageEntities = await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .Where(l => l.UserId == userId)
            .ToListAsync(token);

        return languageEntities
            .Select(l => new LanguageDto
            {
                Id = l.Id, Name = l.Name, Url = l.LanguageDictionary.Url, UserId = l.UserId,
                LanguageDictionaryId = l.LanguageDictionaryId
            })
            .ToList();
    }

    public async Task<LanguageDto> GetByIdAsync(int languageId, CancellationToken token)
    {
        var languageEntity = await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .FirstOrDefaultAsync(l => l.Id == languageId, token);

        if (languageEntity == null) return null;

        return new LanguageDto
        {
            Id = languageEntity.Id,
            Name = languageEntity.Name,
            Url = languageEntity.LanguageDictionary.Url,
            UserId = languageEntity.UserId,
            LanguageDictionaryId = languageEntity.LanguageDictionaryId
        };
    }

    public async Task<LanguageDto> GetByNameAndUserAsync(string name, int userId, CancellationToken token)
    {
        var languageEntity = await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .FirstOrDefaultAsync(l => l.Name == name && l.UserId == userId, token);

        if (languageEntity == null) return null;

        return new LanguageDto
        {
            Id = languageEntity.Id,
            Name = languageEntity.Name,
            Url = languageEntity.LanguageDictionary.Url,
            UserId = languageEntity.UserId,
            LanguageDictionaryId = languageEntity.LanguageDictionaryId
        };
    }

    public async Task AddAsync(LanguageAddDto language, int userId, CancellationToken token)
    {
        var languageEntity = new Language
        {
            Name = language.Name,
            UserId = userId,
            LanguageDictionaryId = language.LanguageDictionaryId
        };

        await _dbContext.Languages.AddAsync(languageEntity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int languageId, CancellationToken token)
    {
        var entity = await _dbContext.Languages
            .FirstOrDefaultAsync(l => l.Id == languageId, token);

        _dbContext.Languages.Remove(entity);
        await _dbContext.SaveChangesAsync(token);
    }
}