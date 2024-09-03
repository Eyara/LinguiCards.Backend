using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class LanguageDictionaryRepository : ILanguageDictionaryRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public LanguageDictionaryRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LanguageDictionaryDto>> GetAllAsync(CancellationToken token)
    {
        var languageDictionaries = await _dbContext.LanguageDictionaries
            .ToListAsync(token);

        return languageDictionaries
            .Select(l => new LanguageDictionaryDto { Id = l.Id, Name = l.Name, Url = l.Url })
            .ToList();
    }
}