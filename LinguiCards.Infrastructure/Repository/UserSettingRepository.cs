using AutoMapper;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class UserSettingRepository : IUserSettingRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserSettingRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddOrUpdateAsync(int userId, int activeTrainingSize, int passiveTrainingSize, int? dailyGoalXp, CancellationToken token)
    {
        var userSetting = await _dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, token);

        if (userSetting != null)
        {
            userSetting.ActiveTrainingSize = activeTrainingSize;
            userSetting.PassiveTrainingSize = passiveTrainingSize;
            userSetting.DailyGoalXp = dailyGoalXp;
        }
        else
        {
            var newSetting = new UserSetting
            {
                UserId = userId,
                ActiveTrainingSize = activeTrainingSize,
                PassiveTrainingSize = passiveTrainingSize,
                DailyGoalXp = dailyGoalXp
            };
            await _dbContext.UserSettings.AddAsync(newSetting, token);
        }

        await _dbContext.SaveChangesAsync(token);
    }


    public async Task<UserSettingDto?> GetByUserIdAsync(int userId, CancellationToken token)
    {
        var entity = await _dbContext.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId, token);

        return entity == null ? null : _mapper.Map<UserSettingDto>(entity);
    }

}