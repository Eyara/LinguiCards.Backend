﻿using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace LinguiCards.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<ILanguageDictionaryRepository, LanguageDictionaryRepository>();
        services.AddScoped<IWordRepository, WordRepository>();
        services.AddScoped<IWordChangeHistoryRepository, WordChangeHistoryRepository>();
        services.AddScoped<IDefaultCribRepository, DefaultCribRepository>();
        services.AddScoped<IUserSettingRepository, UserSettingRepository>();
        services.AddScoped<ITranslationEvaluationHistoryRepository, TranslationEvaluationHistoryRepository>();

        return services;
    }
}