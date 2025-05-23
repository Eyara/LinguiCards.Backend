using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTextForTranslationQuery;

public class GetTextForTranslationQueryHandler : IRequestHandler<GetTextForTranslationQuery, string>
{
    private readonly IOpenAIService _openAiService;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IRedisCacheService _redisCacheService;

    public GetTextForTranslationQueryHandler(IOpenAIService openAiService, ILanguageRepository languageRepository,
        IRedisCacheService redisCacheService, IUsersRepository usersRepository)
    {
        _openAiService = openAiService;
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _redisCacheService = redisCacheService;
    }

    public async Task<string> Handle(GetTextForTranslationQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language = await _languageRepository.GetByIdAsync(request.languageId, cancellationToken);

        if (language == null) throw new LanguageNotFoundException();

        var key = RedisKeyHelper.GetWordOfTheDayKey(user.Id, request.languageId);
        var wordOfTheDay = await _redisCacheService.GetAsync<string>(key);

        return await _openAiService.GetChatResponseAsync(
            TranslationPrompts.GetTextForTranslationPrompt(language.Name, request.Length, request.Level, request.Topic,
                wordOfTheDay));
    }
}