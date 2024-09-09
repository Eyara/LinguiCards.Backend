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

    public async Task AddAsync(int wordId, bool isCorrectAnswer, CancellationToken token)
    {
        await _dbContext.WordChangeHistories.AddAsync(
            new WordChangeHistory
            {
                ChangedOn = DateTime.UtcNow,
                IsCorrectAnswer = isCorrectAnswer,
                WordId = wordId
            }
        );
        await _dbContext.SaveChangesAsync(token);
    }
}