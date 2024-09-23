using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetUnlearnedWordsQuery;

public class GetUnlearnedWordsQueryHandler : IRequestHandler<GetUnlearnedWordsQuery, List<TrainingWord>>
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

    public async Task<List<TrainingWord>> Handle(GetUnlearnedWordsQuery request, CancellationToken cancellationToken)
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

        var words = (List<WordDto>)result.Shuffle();

        return await GetTrainingWords(words, cancellationToken);
    }

    private async Task<List<TrainingWord>> GetTrainingWords(List<WordDto> words, CancellationToken token)
    {
        var count = words.Count;
        var result = new List<TrainingWord>();

        // TODO: add smart split system for different types of training

        for (var i = 0; i < count; i++)
        {
            var type = i < count / 2 ? TrainingType.FromLearnLanguage : TrainingType.FromNativeLanguage;
            var options = await GetTrainingOptions(
                type == TrainingType.FromLearnLanguage ? words[i].TranslatedName : words[i].Name,
                words[i].LanguageId,
                type,
                token);

            result.Add(new TrainingWord
            {
                Id = words[i].Id,
                LanguageId = words[i].LanguageId,
                LastUpdated = words[i].LastUpdated,
                LearnedPercent = words[i].LearnedPercent,
                Name = words[i].Name,
                TranslatedName = words[i].TranslatedName,
                Type = type,
                Options = options
            });
        }

        return result;
    }

    private async Task<List<string>> GetTrainingOptions(string targetOption, int languageId, TrainingType type,
        CancellationToken token)
    {
        var allWords = await _wordRepository.GetAllAsync(languageId, token);

        if (allWords.Count < 3)
        {
            throw new Exception();
        }

        var result = allWords
            .Select(w => type == TrainingType.FromLearnLanguage ? w.TranslatedName : w.Name)
            .OrderBy(o => Guid.NewGuid())
            .Take(3)
            .ToList();
        
        result.Add(targetOption);
        return (List<string>)result.Shuffle();
    }
}