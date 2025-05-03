using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
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

        var isActive = (VocabularyType)historyRecord.VocabularyType == VocabularyType.Active;
        var newLevelPercent = isActive ? historyRecord.ActiveLearned : historyRecord.PassiveLearned;

        newLevelPercent += LearningSettings.LearnStep * 2;

        newLevelPercent = Math.Max(newLevelPercent, 0);
        newLevelPercent = Math.Min(newLevelPercent, 100);

        if (isActive)
            await _wordRepository.UpdateActiveLearnLevel(request.WordId, newLevelPercent,
                cancellationToken);
        else
            await _wordRepository.UpdatePassiveLearnLevel(request.WordId, newLevelPercent,
                cancellationToken);

        return true;
    }
}