﻿using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IDailyGoalRepository
{
    Task AddXpAsync(int userId, int xpDelta, CancellationToken token);
    
    Task SetTotalXpAsync(int userId, int totalXp, int targetXp, CancellationToken token);

    Task<DailyGoalDTO?> GetTodayGoalByUserId(int userId, CancellationToken token);
}