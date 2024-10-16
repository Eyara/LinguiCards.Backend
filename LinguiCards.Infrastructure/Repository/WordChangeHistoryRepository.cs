using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Domain.Entities;

namespace LinguiCards.Infrastructure.Repository;

public class WordChangeHistoryRepository : IWordChangeHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public WordChangeHistoryRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int wordId, bool isCorrectAnswer, int type, double passiveLearned, double activeLearned,
        Guid? trainingId, string? answer, CancellationToken token)
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
                Answer = answer
            }
        );
        await _dbContext.SaveChangesAsync(token);
    }
}