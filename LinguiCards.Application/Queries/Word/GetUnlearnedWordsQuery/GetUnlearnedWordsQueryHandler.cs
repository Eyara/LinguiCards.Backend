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

        var unlearnedPassiveWords = await _wordRepository.GetUnlearned(request.LanguageId,
            LearningSettings.LearnThreshold, VocabularyType.Passive,
            cancellationToken);

        var unlearnedActiveWords = await _wordRepository.GetUnlearned(request.LanguageId,
            LearningSettings.LearnThreshold, VocabularyType.Active,
            cancellationToken);

        // TODO: add method to WordRepo with range update learn level

        await UpdateLearnLevel(
            unlearnedPassiveWords,
            word => word.PassiveLearnedPercent,
            _wordRepository.UpdatePassiveLearnLevel,
            cancellationToken
        );

        await UpdateLearnLevel(
            unlearnedActiveWords,
            word => word.ActiveLearnedPercent,
            _wordRepository.UpdateActiveLearnLevel,
            cancellationToken
        );

        var resultPassive = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            VocabularyType.Passive,
            cancellationToken, 8);

        var resultActive = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            VocabularyType.Active,
            cancellationToken, 8);

        var result = new List<TrainingWord>();

        var trainingId = Guid.NewGuid();

        var trainingWordsPassive =
            await GetTrainingWords(resultPassive, VocabularyType.Passive, trainingId, cancellationToken);
        var trainingWordsActive =
            await GetTrainingWords(resultActive, VocabularyType.Active, trainingId, cancellationToken);

        result.AddRange(trainingWordsPassive);
        result.AddRange(trainingWordsActive);

        return result;
    }

    private async Task<List<TrainingWord>> GetTrainingWords(List<WordDto> words, VocabularyType vocabularyType,
        Guid trainingId,
        CancellationToken token)
    {
        var count = words.Count;
        var result = new List<TrainingWord>();

        for (var i = 0; i < count; i++)
        {
            var type = GetTrainingType(i, count, vocabularyType);
            var options = vocabularyType == VocabularyType.Passive
                ? await GetTrainingOptions(
                    type == TrainingType.FromLearnLanguage ? words[i].TranslatedName : words[i].Name,
                    words[i].LanguageId,
                    type,
                    token)
                : null;

            result.Add(new TrainingWord
            {
                Id = words[i].Id,
                LanguageId = words[i].LanguageId,
                LastUpdated = words[i].LastUpdated,
                PassiveLearnedPercent = words[i].PassiveLearnedPercent,
                Name = words[i].Name,
                TranslatedName = words[i].TranslatedName,
                Type = type,
                Options = options,
                TrainingId = trainingId
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
            .Select(w =>
                type == TrainingType.FromLearnLanguage || type == TrainingType.WritingFromLearnLanguage
                    ? w.TranslatedName
                    : w.Name)
            .Where(option => option != targetOption)
            .OrderBy(o => Guid.NewGuid())
            .Take(3)
            .ToList();

        result.Add(targetOption);
        return (List<string>)result.Shuffle();
    }

    private TrainingType GetTrainingType(int i, int count, VocabularyType vocabularyType)
    {
        if (vocabularyType == VocabularyType.Passive)
        {
            return i < count / 2 ? TrainingType.FromLearnLanguage : TrainingType.FromNativeLanguage;
        }

        return i < count / 2 ? TrainingType.WritingFromLearnLanguage : TrainingType.WritingFromNativeLanguage;
    }

    private async Task UpdateLearnLevel(IEnumerable<WordDto> words, Func<WordDto, double> getLearnedPercent,
        Func<int, double, CancellationToken, Task> updateLearnLevel, CancellationToken cancellationToken)
    {
        foreach (var word in words)
        {
            if (word.LastUpdated.HasValue && word.LastUpdated < DateTime.Today)
            {
                var daysDifference = (DateTime.Today - word.LastUpdated.Value).Days;
                var newLearnedPercent =
                    Math.Max(Math.Round(getLearnedPercent(word) - LearningSettings.DayWeight * daysDifference, 2), 0);

                await updateLearnLevel(word.Id, newLearnedPercent, cancellationToken);
            }
        }
    }
}