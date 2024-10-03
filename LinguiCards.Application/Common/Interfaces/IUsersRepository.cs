using LinguiCards.Domain.Entities;

namespace LinguiCards.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken token);
    Task<User> GetByIdAsync(int id, CancellationToken token);
    Task<User> GetByNameAsync(string name, CancellationToken token);
    Task UpdateXPLevel(double xp, int level, int userId, CancellationToken token);
}