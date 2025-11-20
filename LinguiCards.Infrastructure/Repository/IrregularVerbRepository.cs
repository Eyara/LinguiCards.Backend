using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class IrregularVerbRepository : IIrregularVerbRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public IrregularVerbRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IrregularVerbDto> GetByIdAsync(int irregularVerbId, CancellationToken token)
    {
        var entity = await _dbContext.IrregularVerbs
            .FirstOrDefaultAsync(iv => iv.Id == irregularVerbId, token);

        return entity == null ? null : _mapper.Map<IrregularVerbDto>(entity);
    }

    public async Task<List<IrregularVerbDto>> GetByLanguageAsync(int languageId, CancellationToken token)
    {
        return await _dbContext.IrregularVerbs
            .Where(iv => iv.LanguageId == languageId)
            .ProjectTo<IrregularVerbDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }
}

