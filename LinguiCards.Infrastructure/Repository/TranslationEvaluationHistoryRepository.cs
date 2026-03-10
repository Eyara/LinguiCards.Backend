using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class TranslationEvaluationHistoryRepository : ITranslationEvaluationHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public TranslationEvaluationHistoryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddAsync(TranslationEvaluationHistoryDTO historyRecord, CancellationToken token)
    {
        var entity = _mapper.Map<TranslationEvaluationHistory>(historyRecord);
        await _dbContext.TranslationEvaluationHistories.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<List<TranslationEvaluationHistoryDTO>> GetByLanguageIdAsync(int userId, int? languageId,
        CancellationToken token)
    {
        var query = _dbContext.TranslationEvaluationHistories
            .AsNoTracking()
            .Where(h => h.UserId == userId);

        if (languageId.HasValue)
            query = query.Where(h => h.LanguageId == languageId.Value);

        return await query
            .ProjectTo<TranslationEvaluationHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }
}