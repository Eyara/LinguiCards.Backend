using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class WordChangeHistoryRepository : IWordChangeHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public WordChangeHistoryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddAsync(
        int wordId,
        bool isCorrectAnswer,
        int vocabularyType,
        double passiveLearned,
        double activeLearned,
        Guid? trainingId,
        string? correctAnswer,
        string? answer,
        CancellationToken token)
    {
        var history = new WordChangeHistory
        {
            WordId = wordId,
            IsCorrectAnswer = isCorrectAnswer,
            VocabularyType = vocabularyType,
            PassiveLearned = passiveLearned,
            ActiveLearned = activeLearned,
            TrainingId = trainingId,
            CorrectAnswer = correctAnswer,
            Answer = answer,
            ChangedOn = DateTime.UtcNow
        };

        await _dbContext.WordChangeHistories.AddAsync(history, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<List<WordChangeHistoryDTO>> GetAllByIdAsync(Guid trainingId, CancellationToken token)
    {
        return await _dbContext.WordChangeHistories
            .Where(h => h.TrainingId == trainingId)
            .ProjectTo<WordChangeHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }

}