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

    public async Task AddOrUpdateAsync(int userId, int gainedXp, int targetXp, int? dailyGoalXp,
        CancellationToken token)
    {
        var dailyGoal = await _dbContext.DailyGoals
            .FirstOrDefaultAsync(us => us.UserId == userId, token);

        if (dailyGoal != null)
        {
            dailyGoal.GainedXp = gainedXp;
            dailyGoal.TargetXp = targetXp;
        }
        else
        {
            var newGoal = new DailyGoal
            {
                UserId = userId,
                GainedXp = gainedXp,
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
}