using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IUserSettingRepository
{
    Task AddOrUpdateAsync(int userId, int activeTrainingSize, int passiveTrainingSize, int? dailyGoalXp, CancellationToken token);
    Task<UserSettingDto?> GetByUserIdAsync(int userId, CancellationToken token);
}