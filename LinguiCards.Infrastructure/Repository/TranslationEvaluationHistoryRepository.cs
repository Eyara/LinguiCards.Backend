using AutoMapper;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

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
}