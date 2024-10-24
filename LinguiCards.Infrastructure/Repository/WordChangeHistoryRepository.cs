using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class WordChangeHistoryRepository : IWordChangeHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public WordChangeHistoryRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int wordId, bool isCorrectAnswer, int type, double passiveLearned, double activeLearned,
        Guid? trainingId, string? correctAnswer, string? answer, CancellationToken token)
    {
        await _dbContext.WordChangeHistories.AddAsync(
            new WordChangeHistory
            {
                ChangedOn = DateTime.UtcNow,
                IsCorrectAnswer = isCorrectAnswer,
                VocabularyType = type,
                PassiveLearned = passiveLearned,
                ActiveLearned = activeLearned,
                WordId = wordId,
                TrainingId = trainingId,
                Answer = answer,
                CorrectAnswer = correctAnswer
            }
        );
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<List<WordChangeHistoryDTO>> GetAllById(Guid trainingId, CancellationToken token)
    {
        return await _dbContext.WordChangeHistories
            .Where(h => h.TrainingId == trainingId)
            .Select(h => new WordChangeHistoryDTO
            {
                Id = h.Id,
                IsCorrectAnswer = h.IsCorrectAnswer,
                ActiveLearned = h.ActiveLearned,
                PassiveLearned = h.PassiveLearned,
                VocabularyType = h.VocabularyType,
                ChangedOn = h.ChangedOn,
                Answer = h.Answer,
                CorrectAnswer = h.CorrectAnswer
            })
            .ToListAsync(token);
    }
}