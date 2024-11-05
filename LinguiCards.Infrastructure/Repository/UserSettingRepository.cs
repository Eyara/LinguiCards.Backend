using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class UserSettingRepository : IUserSettingRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public UserSettingRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOrUpdateAsync(int userId, int activeTrainingSize, int passiveTrainingSize,
        CancellationToken token)
    {
        var userSetting = await _dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, token);

        if (userSetting != null)
        {
            userSetting.ActiveTrainingSize = activeTrainingSize;
            userSetting.PassiveTrainingSize = passiveTrainingSize;
            _dbContext.UserSettings.Update(userSetting);
        }
        else
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                ActiveTrainingSize = activeTrainingSize,
                PassiveTrainingSize = passiveTrainingSize
            };
            await _dbContext.UserSettings.AddAsync(userSetting, token);
        }

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<UserSettingDto?> GetByUserIdAsync(int userId, CancellationToken token)
    {
        return await _dbContext.UserSettings
            .Where(us => us.UserId == userId)
            .Select(us => new UserSettingDto
            {
                Id = us.Id,
                UserId = us.UserId,
                ActiveTrainingSize = us.ActiveTrainingSize,
                PassiveTrainingSize = us.PassiveTrainingSize
            })
            .FirstOrDefaultAsync(token);
    }
}