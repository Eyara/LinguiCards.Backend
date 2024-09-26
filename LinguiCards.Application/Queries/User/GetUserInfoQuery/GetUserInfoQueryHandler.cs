using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LinguiCards.Application.Queries.User.GetUserInfoQuery;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfo>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public GetUserInfoQueryHandler(IUsersRepository usersRepository, ILanguageRepository languageRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var info = new UserInfo
        {
            LanguageStats = new List<LanguageStat>()
        };

        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        var userLanguages = await _languageRepository.GetAllAsync(user.Id, cancellationToken);

        foreach (var language in userLanguages)
        {
            var words = await _wordRepository.GetAllExtendedAsync(language.Id, cancellationToken);
            var learnedCount = words.Count(w => w.LearnedPercent > LearningSettings.LearnThreshold);
            var trainingDays = words
                .SelectMany(w => w.Histories)
                .Select(h => h.ChangedOn.Date)
                .GroupBy(d => d.Date)
                .Count();
            
            info.LanguageStats.Add(new LanguageStat
            {
                LanguageName = language.Name,
                TotalWords = words.Count,
                LearnedWords = learnedCount,
                LearnedPercent = words.Count > 0 ? Math.Round(learnedCount / (double)words.Count * 100, 2) : 0,
                TotalTrainingDays = trainingDays
            });
        }

        return info;
    }
}