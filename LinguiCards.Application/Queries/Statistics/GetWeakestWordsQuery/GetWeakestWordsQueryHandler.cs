using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetWeakestWordsQuery;

public class GetWeakestWordsQueryHandler
    : IRequestHandler<GetWeakestWordsQuery, WeakestWordsResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;
    private readonly IWordChangeHistoryRepository _historyRepository;

    public GetWeakestWordsQueryHandler(
        IUsersRepository usersRepository,
        IWordRepository wordRepository,
        IWordChangeHistoryRepository historyRepository)
    {
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _historyRepository = historyRepository;
    }

    public async Task<WeakestWordsResponse> Handle(GetWeakestWordsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);
        if (user == null) throw new UserNotFoundException();

        var groupedHistories = await _historyRepository.GetGroupedByWordAsync(
            request.LanguageId, request.MinAttempts, cancellationToken);

        var words = await _wordRepository.GetAllAsync(request.LanguageId, cancellationToken);
        var wordLookup = words.ToDictionary(w => w.Id);

        var weakWords = groupedHistories
            .Select(kvp =>
            {
                var total = kvp.Value.Count;
                var correct = kvp.Value.Count(h => h.IsCorrectAnswer);
                var accuracy = Math.Round(correct / (double)total * 100, 2);
                wordLookup.TryGetValue(kvp.Key, out var word);

                return new WeakWord
                {
                    WordId = kvp.Key,
                    Name = word?.Name ?? string.Empty,
                    TranslatedName = word?.TranslatedName ?? string.Empty,
                    Accuracy = accuracy,
                    TotalAttempts = total,
                    CorrectAttempts = correct,
                    LastAttempted = kvp.Value.Max(h => h.ChangedOn),
                    PassiveLearnedPercent = word?.PassiveLearnedPercent ?? 0,
                    ActiveLearnedPercent = word?.ActiveLearnedPercent ?? 0
                };
            })
            .Where(w => w.Accuracy < 70)
            .OrderBy(w => w.Accuracy)
            .Take(request.Count)
            .ToList();

        var result = new WeakestWordsResponse();

        foreach (var word in weakWords)
        {
            if (word.Accuracy < 30)
                result.Critical.Add(word);
            else if (word.Accuracy < 50)
                result.Weak.Add(word);
            else
                result.NeedsPractice.Add(word);
        }

        return result;
    }
}
