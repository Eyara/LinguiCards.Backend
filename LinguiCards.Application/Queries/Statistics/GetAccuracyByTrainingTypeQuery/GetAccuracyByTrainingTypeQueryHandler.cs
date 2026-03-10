using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetAccuracyByTrainingTypeQuery;

public class GetAccuracyByTrainingTypeQueryHandler
    : IRequestHandler<GetAccuracyByTrainingTypeQuery, AccuracyByTrainingTypeResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IWordChangeHistoryRepository _wordHistoryRepository;
    private readonly IGrammarTaskHistoryRepository _grammarHistoryRepository;
    private readonly ITranslationEvaluationHistoryRepository _translationHistoryRepository;

    public GetAccuracyByTrainingTypeQueryHandler(
        IUsersRepository usersRepository,
        IWordChangeHistoryRepository wordHistoryRepository,
        IGrammarTaskHistoryRepository grammarHistoryRepository,
        ITranslationEvaluationHistoryRepository translationHistoryRepository)
    {
        _usersRepository = usersRepository;
        _wordHistoryRepository = wordHistoryRepository;
        _grammarHistoryRepository = grammarHistoryRepository;
        _translationHistoryRepository = translationHistoryRepository;
    }

    public async Task<AccuracyByTrainingTypeResponse> Handle(GetAccuracyByTrainingTypeQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        DateTime? from = request.RangeDays.HasValue
            ? DateTime.UtcNow.Date.AddDays(-request.RangeDays.Value)
            : null;

        var wordHistories = await _wordHistoryRepository.GetByLanguageIdAsync(
            request.LanguageId, from, cancellationToken);

        var grammarHistories = await _grammarHistoryRepository.GetByLanguageIdAsync(
            user.Id, request.LanguageId, from, cancellationToken);

        var translationHistories = await _translationHistoryRepository.GetByLanguageIdAsync(
            user.Id, request.LanguageId, cancellationToken);

        var passive = wordHistories.Where(h => h.VocabularyType == (int)VocabularyType.Passive).ToList();
        var active = wordHistories.Where(h => h.VocabularyType == (int)VocabularyType.Active).ToList();

        return new AccuracyByTrainingTypeResponse
        {
            PassiveWordTraining = BuildTrainingAccuracy(passive),
            ActiveWordTraining = BuildTrainingAccuracy(active),
            Grammar = BuildAggregateAccuracy(grammarHistories.Select(h => h.Accuracy).ToList()),
            TranslationEvaluation = BuildAggregateAccuracy(translationHistories.Select(h => h.Accuracy).ToList())
        };
    }

    private static TrainingAccuracy BuildTrainingAccuracy(List<WordChangeHistoryDTO> histories)
    {
        if (histories.Count == 0)
            return new TrainingAccuracy();

        var correct = histories.Count(h => h.IsCorrectAnswer);
        return new TrainingAccuracy
        {
            TotalAttempts = histories.Count,
            CorrectAttempts = correct,
            Accuracy = Math.Round(correct / (double)histories.Count * 100, 2)
        };
    }

    private static AggregateAccuracy BuildAggregateAccuracy(List<int> accuracies)
    {
        if (accuracies.Count == 0)
            return new AggregateAccuracy();

        return new AggregateAccuracy
        {
            TotalAttempts = accuracies.Count,
            AverageAccuracy = Math.Round(accuracies.Average(), 2)
        };
    }
}
