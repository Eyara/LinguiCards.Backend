using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class LanguageDictionaryRepository : ILanguageDictionaryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public LanguageDictionaryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<LanguageDictionaryDto>> GetAllAsync(CancellationToken token)
    {
        return await _dbContext.LanguageDictionaries
            .ProjectTo<LanguageDictionaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }
}