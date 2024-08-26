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
            .Where(l => l.UserId == userId)
            .ToListAsync(token);

        return languageEntities
            .Select(l => new LanguageDto { Id = l.Id, Name = l.Name, FlagUrl = l.FlagUrl, UserId = l.UserId })
            .ToList();
    }

    public async Task<LanguageDto> GetByIdAsync(int languageId, CancellationToken token)
    {
        var languageEntity = await _dbContext.Languages
            .FirstOrDefaultAsync(l => l.Id == languageId, token);

        if (languageEntity == null) return null;

        return new LanguageDto
        {
            Id = languageEntity.Id, Name = languageEntity.Name, FlagUrl = languageEntity.FlagUrl,
            UserId = languageEntity.UserId
        };
    }

    public async Task AddAsync(LanguageDto language, int userId, CancellationToken token)
    {
        var languageEntity = new Language
        {
            Name = language.Name,
            FlagUrl = language.FlagUrl,
            UserId = userId
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