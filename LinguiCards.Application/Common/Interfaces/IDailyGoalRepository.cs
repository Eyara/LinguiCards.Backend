using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IDailyGoalRepository
{
    Task AddXpAsync(int userId, int xpDelta, int targetXp, CancellationToken token);
    
    Task SetTotalXpAsync(int userId, int totalXp, int targetXp, CancellationToken token);

    Task AddXpAndAddToByTranslationAsync(int userId, int xpDelta, int targetXp, CancellationToken token);
    
    Task AddXpAndAddToByGrammarAsync(int userId, int xpDelta, int targetXp, CancellationToken token);

    Task<DailyGoalDTO?> GetTodayGoalByUserId(int userId, CancellationToken token);
    
    Task<List<GoalDay>> GetGoalDaysByUserIdAsync(int userId, CancellationToken token);
    
    Task<int> GetGoalStreakByUserIdAsync(int userId, CancellationToken token);
}