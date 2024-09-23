using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetUnlearnedWordsQuery;

public class GetUnlearnedWordsQueryHandler : IRequestHandler<GetUnlearnedWordsQuery, List<WordDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    private const double DegradingRate = 0.3d;

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

        if (user == null) throw new UserNotFoundException();

        var languageEntity = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (languageEntity == null) throw new LanguageNotFoundException();

        if (languageEntity.UserId != user.Id) throw new EntityOwnershipException();
        
        var unlearnedWords = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            cancellationToken);
        
        // TODO: add method to WordRepo with range update learn level

        foreach (var word in unlearnedWords)
        {
            if (word.LastUpdated.HasValue && word.LastUpdated < DateTime.Today)
            {
                await _wordRepository.UpdateLearnLevel(
                    word.Id, 
                    Math.Round(word.LearnedPercent - DegradingRate * (word.LastUpdated.Value - DateTime.Today).Days, 2),
                    cancellationToken);
            }
        }
        
        var result = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            cancellationToken);

        return (List<WordDto>)result.Shuffle();
    }
}