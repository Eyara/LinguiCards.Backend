using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Queries.Language.GetLanguageStatsQuery;

public class GetLanguageStatsQueryHandler : IRequestHandler<GetLanguageStatsQuery, LanguageDashboardStat>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public GetLanguageStatsQueryHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<LanguageDashboardStat> Handle(GetLanguageStatsQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        var words = await _wordRepository.GetAllExtendedAsync(request.LanguageId, cancellationToken);
        var totalWords = words.Count;
        var totalLearnedWords = words.Count(word =>
            word.ActiveLearnedPercent >= LearningSettings.LearnThreshold &&
            word.PassiveLearnedPercent >= LearningSettings.LearnThreshold);

        var learnedActiveCount = GetLearnedWordCount(words, w => w.ActiveLearnedPercent);
        var learnedPassiveCount = GetLearnedWordCount(words, w => w.PassiveLearnedPercent);
        var trainingDays = GetDistinctTrainingDays(words);

        var averageActiveAccuracy = CalculateAverageAccuracy(words, VocabularyType.Active);
        var averagePassiveAccuracy = CalculateAverageAccuracy(words, VocabularyType.Passive);

        var activeAverageLearnedPercent = Math.Round(words.Sum(word => word.ActiveLearnedPercent) / words.Count, 2);
        var passiveAverageLearnedPercent = Math.Round(words.Sum(word => word.PassiveLearnedPercent) / words.Count, 2);

        return new LanguageDashboardStat
        {
            TotalWords = totalWords,
            LearnedWords = totalLearnedWords,
            TotalTrainingDays = trainingDays,
            ActiveLearnedPercent = CalculateLearnedPercent(learnedActiveCount, totalWords),
            PassiveLearnedPercent = CalculateLearnedPercent(learnedPassiveCount, totalWords),
            ActiveAverageAccuracy = averageActiveAccuracy,
            PassiveAverageAccuracy = averagePassiveAccuracy,
            ActiveAverageLearnedPercent = activeAverageLearnedPercent,
            PassiveAverageLearnedPercent = passiveAverageLearnedPercent,
            BestActiveWordsByAccuracy = GetTopWordsByAccuracy(words, VocabularyType.Active, true),
            WorstActiveWordsByAccuracy = GetTopWordsByAccuracy(words, VocabularyType.Active, false),
            BestPassiveWordsByAccuracy = GetTopWordsByAccuracy(words, VocabularyType.Passive, true),
            WorstPassiveWordsByAccuracy = GetTopWordsByAccuracy(words, VocabularyType.Passive, false),
        };
    }

    private int GetLearnedWordCount(IEnumerable<WordDto> words, Func<WordDto, double> learnedPercentSelector)
    {
        return words.Count(w => learnedPercentSelector(w) > LearningSettings.LearnThreshold);
    }

    private int GetDistinctTrainingDays(IEnumerable<WordExtendedDTO> words)
    {
        return words
            .SelectMany(w => w.Histories)
            .Select(h => h.ChangedOn.Date)
            .Distinct()
            .Count();
    }

    private double CalculateAverageAccuracy(IEnumerable<WordExtendedDTO> words, VocabularyType vocabularyType)
    {
        var relevantHistories = words
            .SelectMany(w => w.Histories)
            .Where(h => h.VocabularyType == (int)vocabularyType)
            .ToList();

        if (!relevantHistories.Any()) return 0;

        return Math.Round(
            relevantHistories.Count(h => h.IsCorrectAnswer) / (double)relevantHistories.Count, 2);
    }

    private double CalculateLearnedPercent(int learnedCount, int totalWords)
    {
        return totalWords > 0 ? Math.Round(learnedCount / (double)totalWords, 2) : 0;
    }
    
    private List<string> GetTopWordsByAccuracy(IEnumerable<WordExtendedDTO> words, VocabularyType vocabularyType, bool isBest)
    {
        var wordAccuracyList = words
            .Where(w => w.Histories.Any(h => h.VocabularyType == (int)vocabularyType))
            .Select(w => new
            {
                Word = w,
                Accuracy = CalculateWordAccuracy(w.Histories, vocabularyType)
            })
            .ToList();

        var bestWords = wordAccuracyList
            .OrderByDescending(a => a.Accuracy)
            .Take(3)
            .Select(a => a.Word.Name)
            .ToList();

        if (!isBest)
        {
            var worstWords = wordAccuracyList
                .OrderBy(a => a.Accuracy)
                .Where(a => !bestWords.Contains(a.Word.Name))
                .Take(3)
                .Select(a => a.Word.Name)
                .ToList();

            return worstWords;
        }

        return bestWords;
    }

    
    private double CalculateWordAccuracy(IEnumerable<WordChangeHistoryDTO> histories, VocabularyType vocabularyType)
    {
        var relevantHistories = histories
            .Where(h => h.VocabularyType == (int)vocabularyType)
            .ToList();

        if (!relevantHistories.Any()) return 0;

        return relevantHistories.Count(h => h.IsCorrectAnswer) / (double)relevantHistories.Count;
    }
}