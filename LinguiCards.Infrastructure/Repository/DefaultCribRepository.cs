using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class DefaultCribRepository : IDefaultCribRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public DefaultCribRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CribDTO>> GetAllAsync(int languageId, CancellationToken token)
    {
        var result = await (from cr in _dbContext.DefaultCribs
                join cd in _dbContext.CribDescriptions
                    on cr.Id equals cd.CribId
                where cd.Type == 0 && cr.LanguageId == languageId
                select new 
                {
                    CribId = cr.Id,
                    cr.LanguageId,
                    cd.Id,
                    cd.Header,
                    cd.Description,
                    cd.Order,
                    cd.Type
                })
            .ToListAsync();

        var groupedResult = result
            .GroupBy(x => new { x.CribId, x.LanguageId })
            .Select(g => new CribDTO
            {
                Id = g.Key.CribId,
                Languageid = g.Key.LanguageId,
                CribDescriptions = g.Select(cd => new CribDescriptionDTO
                {
                    Id = cd.Id,
                    Header = cd.Header,
                    Description = cd.Description,
                    Order = cd.Order,
                    Type = cd.Type
                }).ToList()
            }).ToList();

        return groupedResult;
    }
}