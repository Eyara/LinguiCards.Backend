using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class UsersRepository : IUsersRepository
{
    private readonly LinguiCardsDbContext _dbContext;

    public UsersRepository(LinguiCardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user, CancellationToken token)
    {
        await _dbContext.AddAsync(user, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken token)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, token);
    }

    public async Task<User> GetByNameAsync(string name, CancellationToken token)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == name, token);
    }

    public async Task UpdateXPLevel(double xp, int level, int userId, CancellationToken token)
    {
        var user = await GetByIdAsync(userId, token);
        user.XP = xp;
        user.Level = level;

        await _dbContext.SaveChangesAsync(token);
    }
}