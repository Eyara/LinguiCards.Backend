using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace LinguiCards.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();

        return services;
    }
}