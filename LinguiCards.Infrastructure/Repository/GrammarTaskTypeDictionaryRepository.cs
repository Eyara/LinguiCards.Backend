using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class GrammarTaskTypeDictionaryRepository : IGrammarTaskTypeDictionaryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GrammarTaskTypeDictionaryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<GrammarTaskTypeDictionaryDto>> GetAllAsync(CancellationToken token)
    {
        return await _dbContext.GrammarTaskTypeDictionary
            .ProjectTo<GrammarTaskTypeDictionaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }
}

