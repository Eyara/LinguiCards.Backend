using AutoMapper;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class DailyGoalRepository : IDailyGoalRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public DailyGoalRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddXpAsync(int userId, int xpDelta, int targetXp, CancellationToken token)
    {
        var dailyGoal = await _dbContext.DailyGoals
            .FirstOrDefaultAsync(dg => dg.UserId == userId && dg.Date == DateOnly.FromDateTime(DateTime.Now), token);

        if (dailyGoal != null)
        {
            dailyGoal.GainedXp += xpDelta;
        }
        else
        {
            var newGoal = new DailyGoal
            {
                UserId = userId,
                GainedXp = xpDelta,
                TargetXp = targetXp,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };
            await _dbContext.DailyGoals.AddAsync(newGoal, token);
        }

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task SetTotalXpAsync(int userId, int totalXp, int targetXp, CancellationToken token)
    {
        var dailyGoal = await _dbContext.DailyGoals
            .FirstOrDefaultAsync(dg => dg.UserId == userId && dg.Date == DateOnly.FromDateTime(DateTime.Now), token);

        if (dailyGoal != null)
        {
            if (totalXp > dailyGoal.GainedXp)
            {
                dailyGoal.GainedXp = totalXp;
            }
            dailyGoal.TargetXp = targetXp;
        }
        else
        {
            var newGoal = new DailyGoal
            {
                UserId = userId,
                GainedXp = totalXp,
                TargetXp = targetXp,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };
            await _dbContext.DailyGoals.AddAsync(newGoal, token);
        }

        await _dbContext.SaveChangesAsync(token);
    }


    public async Task<DailyGoalDTO?> GetTodayGoalByUserId(int userId, CancellationToken token)
    {
        var entity = await _dbContext.DailyGoals
            .AsNoTracking()
            .FirstOrDefaultAsync(dg => dg.UserId == userId && dg.Date == DateOnly.FromDateTime(DateTime.Now), token);

        return entity == null ? null : _mapper.Map<DailyGoalDTO>(entity);
    }

    public async Task<List<DateOnly>> GetCompletedGoalDaysByUserIdAsync(int userId, CancellationToken token)
    {
        var completedGoals = await _dbContext.DailyGoals
            .AsNoTracking()
            .Where(dg => dg.UserId == userId && dg.GainedXp >= dg.TargetXp)
            .OrderByDescending(dg => dg.Date)
            .Select(dg => dg.Date)
            .ToListAsync(token);

        return completedGoals;
    }

    public async Task<int> GetGoalStreakByUserIdAsync(int userId, CancellationToken token)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        var completedGoals = await _dbContext.DailyGoals
            .AsNoTracking()
            .Where(dg => dg.UserId == userId && dg.GainedXp >= dg.TargetXp)
            .OrderByDescending(dg => dg.Date)
            .Select(dg => dg.Date)
            .ToListAsync(token);

        if (completedGoals.Count == 0)
        {
            return 0;
        }

        var completedDatesSet = completedGoals.ToHashSet();
        var streak = 0;
        DateOnly currentDate;

        if (completedDatesSet.Contains(today))
        {
            currentDate = today;
        }
        else
        {
            currentDate = today.AddDays(-1);
        }

        while (completedDatesSet.Contains(currentDate))
        {
            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }
}