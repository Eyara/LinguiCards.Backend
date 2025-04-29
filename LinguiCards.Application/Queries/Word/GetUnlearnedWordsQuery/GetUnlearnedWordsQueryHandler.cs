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
    private readonly IUserSettingRepository _userSettingRepository;

    public GetUnlearnedWordsQueryHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository, IUserSettingRepository userSettingRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _userSettingRepository = userSettingRepository;
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

        await UpdateLearnLevel(
            unlearnedPassiveWords,
            VocabularyType.Passive,
            cancellationToken
        );

        await UpdateLearnLevel(
            unlearnedActiveWords,
            VocabularyType.Active,
            cancellationToken
        );

        var (activeTrainingSize, passiveTrainingSize) = await GetTrainingSizes(user.Id, cancellationToken);

        var resultPassive = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            VocabularyType.Passive,
            cancellationToken, passiveTrainingSize);

        var resultActive = await _wordRepository.GetUnlearned(request.LanguageId, LearningSettings.LearnThreshold,
            VocabularyType.Active,
            cancellationToken, activeTrainingSize);

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

        if (words.Count == 0) return result;

        var allWords = await _wordRepository.GetAllAsync(words.First().LanguageId, token);

        for (var i = 0; i < count; i++)
        {
            var type = GetTrainingType(i, count, vocabularyType);
            List<string>? options = null;
            List<string>? connectionTargets = null;
            List<WordConnection> connectionMatches = new List<WordConnection>();

            if (vocabularyType == VocabularyType.Passive)
            {
                if (type == TrainingType.WordConnect)
                {
                    var randomWords = allWords
                        .Where(w => w.Id != words[i].Id)
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(3)
                        .ToList();

                    connectionTargets = randomWords.Select(w => w.Name).Distinct().ToList();
                    connectionTargets.Add(words[i].Name);
                    connectionTargets = connectionTargets.OrderBy(_ => Guid.NewGuid()).ToList();

                    connectionMatches.Add(new WordConnection()
                    {
                        Id = words[i].Id,
                        Type = type,
                        Name = words[i].Name,
                        TranslatedName = words[i].TranslatedName,
                        TrainingId = trainingId
                    });
                    foreach (var randomWord in randomWords)
                    {
                        connectionMatches.Add(new WordConnection()
                        {
                            Id = randomWord.Id,
                            Type = type,
                            Name = randomWord.Name,
                            TranslatedName = randomWord.TranslatedName,
                            TrainingId = trainingId
                        });
                    }

                    options = randomWords.Select(w => w.TranslatedName).ToList();
                    options.Add(words[i].TranslatedName);
                    options = options.OrderBy(_ => Guid.NewGuid()).ToList();
                }
                else
                {
                    options = await GetTrainingOptions(
                        type == TrainingType.FromLearnLanguage ? words[i].TranslatedName : words[i].Name,
                        words[i].LanguageId,
                        type,
                        token);
                }
            }

            var modifiedName = words[i].Name;
            var modifiedTranslatedName = words[i].TranslatedName;
            var filteredWords = allWords.Where(w => w.Id != words[i].Id).ToList();

            switch (type)
            {
                // Adjust name or translated name if there is a collision in allWords
                case TrainingType.FromLearnLanguage:
                case TrainingType.WritingFromLearnLanguage:
                    modifiedName = ResolveNameConflict(words[i].Name, words[i].TranslatedName, filteredWords, true);
                    break;
                case TrainingType.FromNativeLanguage:
                case TrainingType.WritingFromNativeLanguage:
                    modifiedTranslatedName =
                        ResolveNameConflict(words[i].TranslatedName, words[i].Name, filteredWords, false);
                    break;
                case TrainingType.Sentence:
                    break;
                case TrainingType.WordConnect:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            result.Add(new TrainingWord
            {
                Id = words[i].Id,
                LanguageId = words[i].LanguageId,
                Name = modifiedName,
                TranslatedName = modifiedTranslatedName,
                Type = type,
                Options = options,
                ConnectionTargets = connectionTargets,
                ConnectionMatches = connectionMatches,
                TrainingId = trainingId
            });
        }

        return result;
    }

    private async Task<List<string>> GetTrainingOptions(string targetOption, int languageId, TrainingType type,
        CancellationToken token)
    {
        var allWords = await _wordRepository.GetAllAsync(languageId, token);

        // TODO: make-up exception
        if (allWords.Count < 3) throw new Exception();

        var result = allWords
            .Select(w =>
                type == TrainingType.FromLearnLanguage || type == TrainingType.WritingFromLearnLanguage
                    ? w.TranslatedName
                    : w.Name)
            .Where(option => option != targetOption)
            .Distinct()
            .OrderBy(option => LevenshteinDistanceHelper.CalculateDistance(option, targetOption))
            .Take(3)
            .ToList();

        result.Add(targetOption);
        return (List<string>)result.Shuffle();
    }

    private TrainingType GetTrainingType(int i, int count, VocabularyType vocabularyType)
    {
        if (vocabularyType == VocabularyType.Passive)
        {
            var fromLearnLangBoundary = count * 40 / 100;
            var fromNativeLangBoundary = fromLearnLangBoundary + count * 40 / 100;

            if (i < fromLearnLangBoundary)
                return TrainingType.FromLearnLanguage;
            if (i < fromNativeLangBoundary)
                return TrainingType.FromNativeLanguage;

            return TrainingType.WordConnect; // Remaining 20%
        }

        // Default case for active vocabulary
        return i < count / 2 ? TrainingType.WritingFromLearnLanguage : TrainingType.WritingFromNativeLanguage;
    }


    private async Task UpdateLearnLevel(IEnumerable<WordDto> words, VocabularyType vocabularyType,
        CancellationToken cancellationToken)
    {
        var wordUpdates = new List<(int wordId, double passivePercent, double activePercent)>();

        foreach (var word in words)
            if (ShouldUpdate(word))
            {
                var daysDifference = GetDaysDifference(word);
                var (newPassivePercent, newActivePercent) =
                    CalculateNewLearnedPercent(word, daysDifference, vocabularyType);

                wordUpdates.Add((word.Id, newPassivePercent, newActivePercent));
            }

        if (wordUpdates.Any()) await _wordRepository.UpdateLearnedPercentRangeAsync(wordUpdates, cancellationToken);
    }

    private bool ShouldUpdate(WordDto word)
    {
        return word.LastUpdated.HasValue && word.LastUpdated < DateTime.Today;
    }

    private int GetDaysDifference(WordDto word)
    {
        return (DateTime.Today - word.LastUpdated.Value).Days;
    }

    private (double newPassivePercent, double newActivePercent) CalculateNewLearnedPercent(WordDto word,
        int daysDifference, VocabularyType vocabularyType)
    {
        var newPassiveLearnedPercent = vocabularyType == VocabularyType.Active
            ? word.PassiveLearnedPercent
            : Math.Max(Math.Round(word.PassiveLearnedPercent - LearningSettings.DayWeight * daysDifference, 2), 0);

        var newActiveLearnedPercent = vocabularyType == VocabularyType.Active
            ? Math.Max(Math.Round(word.ActiveLearnedPercent - LearningSettings.DayWeight * daysDifference, 2), 0)
            : word.ActiveLearnedPercent;

        return (newPassiveLearnedPercent, newActiveLearnedPercent);
    }

    private async Task<(int activeTrainingSize, int passiveTrainingSize)> GetTrainingSizes(int userId,
        CancellationToken token)
    {
        var userSettings = await _userSettingRepository.GetByUserIdAsync(userId, token);

        return userSettings != null ? (userSettings.ActiveTrainingSize, userSettings.PassiveTrainingSize) : (8, 8);
    }

    private string ResolveNameConflict(string primaryName, string fallbackName, List<WordDto> allWords,
        bool checkPrimary)
    {
        var existingWord =
            allWords.FirstOrDefault(w => checkPrimary ? w.Name == primaryName : w.TranslatedName == primaryName);
        if (existingWord == null)
        {
            return primaryName;
        }

        var length = Math.Min(primaryName.Length, fallbackName.Length);
        var modifiedName = primaryName;

        for (int i = 1; i < length; i++)
        {
            var substring = fallbackName.Substring(0, i);
            modifiedName = $"{primaryName} ({substring})";

            var isUnique = !allWords.Any(w => checkPrimary ? w.Name == modifiedName : w.TranslatedName == modifiedName);
            if (isUnique)
            {
                return modifiedName;
            }
        }

        return primaryName;
    }
}