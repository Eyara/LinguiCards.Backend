using AutoMapper;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

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
}

