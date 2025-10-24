using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IDailyGoalRepository
{
    Task AddOrUpdateAsync(int userId, int gainedXp, int targetXp, CancellationToken token);

    Task<DailyGoalDTO?> GetTodayGoalByUserId(int userId, CancellationToken token);
}