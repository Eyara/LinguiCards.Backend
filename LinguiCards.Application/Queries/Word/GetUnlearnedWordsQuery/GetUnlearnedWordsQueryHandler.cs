using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetUnlearnedWordsQuery;

public class GetUnlearnedWordsQueryHandler : IRequestHandler<GetUnlearnedWordsQuery, List<WordDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public GetUnlearnedWordsQueryHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<List<WordDto>> Handle(GetUnlearnedWordsQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new Exception();

        var languageEntity = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (languageEntity == null) throw new Exception();

        if (languageEntity.UserId != user.Id) throw new Exception();

        // TODO: add degrading of learn percent over time

        return await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            cancellationToken);
    }
}