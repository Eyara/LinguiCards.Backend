using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.Word.ChallengeAnswerCommand;

public class ChallengeAnswerCommandHandler : IRequestHandler<ChallengeAnswerCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordChangeHistoryRepository _wordChangeHistoryRepository;
    private readonly IWordRepository _wordRepository;

    public ChallengeAnswerCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository, IWordChangeHistoryRepository wordChangeHistoryRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _wordChangeHistoryRepository = wordChangeHistoryRepository;
    }

    public async Task<bool> Handle(ChallengeAnswerCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var wordEntity = await _wordRepository.GetByIdExtendedAsync(request.WordId, cancellationToken);

        if (wordEntity == null) throw new WordNotFoundException();

        var languageEntity = await _languageRepository.GetByIdAsync(wordEntity.LanguageId, cancellationToken);

        if (languageEntity == null) throw new LanguageNotFoundException();

        if (languageEntity.UserId != user.Id) throw new EntityOwnershipException();

        var historyRecord = wordEntity.Histories.First(wh => wh.TrainingId == request.TrainingId);
        var vocabularyType = (VocabularyType)historyRecord.VocabularyType;
        var isActive = vocabularyType == VocabularyType.Active;

        var currentEf = isActive ? wordEntity.ActiveEaseFactor : wordEntity.PassiveEaseFactor;
        var currentInterval = isActive ? wordEntity.ActiveIntervalDays : wordEntity.PassiveIntervalDays;
        var currentReps = isActive ? wordEntity.ActiveRepetitionCount : wordEntity.PassiveRepetitionCount;

        if (currentEf < SrsCalculator.MinEaseFactor)
            currentEf = SrsCalculator.DefaultEaseFactor;

        const int challengeQuality = 4;
        var srs = SrsCalculator.Calculate(currentEf, currentInterval, currentReps, challengeQuality);
        var nextReview = DateTime.UtcNow.Date.AddDays(srs.IntervalDays);
        var learnedPercent = SrsCalculator.DeriveLearnedPercent(srs.RepetitionCount, srs.IntervalDays);

        await _wordRepository.UpdateSrsState(request.WordId, vocabularyType, srs.EaseFactor,
            srs.IntervalDays, srs.RepetitionCount, nextReview, learnedPercent, cancellationToken);

        return true;
    }
}