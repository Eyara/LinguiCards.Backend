using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class GrammarTaskHistoryRepository : IGrammarTaskHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GrammarTaskHistoryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddAsync(GrammarTaskHistoryDTO historyRecord, CancellationToken token)
    {
        var entity = _mapper.Map<GrammarTaskHistory>(historyRecord);
        await _dbContext.GrammarTaskHistories.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<List<GrammarTaskHistoryDTO>> GetByLanguageIdAsync(int userId, int? languageId,
        DateTime? from, CancellationToken token)
    {
        var query = _dbContext.GrammarTaskHistories
            .AsNoTracking()
            .Where(h => h.UserId == userId);

        if (languageId.HasValue)
            query = query.Where(h => h.LanguageId == languageId.Value);

        if (from.HasValue)
            query = query.Where(h => h.CreatedAt >= from.Value);

        return await query
            .ProjectTo<GrammarTaskHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }
}

