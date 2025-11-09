using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Queries.GrammarTask.GetGrammarTaskQuery;

public class GetGrammarTaskQueryHandler : IRequestHandler<GetGrammarTaskQuery, string>
{
    private readonly IOpenAIService _openAiService;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;

    public GetGrammarTaskQueryHandler(IOpenAIService openAiService, ILanguageRepository languageRepository,
        IUsersRepository usersRepository)
    {
        _openAiService = openAiService;
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
    }

    public async Task<string> Handle(GetGrammarTaskQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var language = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (language == null) throw new LanguageNotFoundException();

        return await _openAiService.GetChatResponseAsync(
            GrammarTaskPrompts.GetGrammarTaskPrompt(language.Name, request.Level, request.Topic, request.Type));
    }
}

