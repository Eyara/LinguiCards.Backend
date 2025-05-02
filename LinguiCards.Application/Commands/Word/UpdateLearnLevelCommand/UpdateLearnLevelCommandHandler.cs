using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;

public class UpdateLearnLevelCommandHandler : IRequestHandler<UpdateLearnLevelCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordChangeHistoryRepository _wordChangeHistoryRepository;
    private readonly IWordRepository _wordRepository;

    public UpdateLearnLevelCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository, IWordChangeHistoryRepository wordChangeHistoryRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _wordChangeHistoryRepository = wordChangeHistoryRepository;
    }

    public async Task<bool> Handle(UpdateLearnLevelCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var wordEntity = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (wordEntity == null) throw new WordNotFoundException();

        var languageEntity = await _languageRepository.GetByIdAsync(wordEntity.LanguageId, cancellationToken);

        if (languageEntity == null) throw new LanguageNotFoundException();

        if (languageEntity.UserId != user.Id) throw new EntityOwnershipException();

        var vocabularyType = TrainingToVocabulary.GetVocabularyType(request.TrainingType);

        var isActive = vocabularyType == VocabularyType.Active;

        var learnedPercent = isActive ? wordEntity.ActiveLearnedPercent : wordEntity.PassiveLearnedPercent;

        var newLevelPercent = request.WasSuccessful
            ? learnedPercent + LearningSettings.LearnStep
            : learnedPercent - LearningSettings.LearnStep;

        var hintCount = request.HintCount ?? 0;
        if (isActive)
        {
            newLevelPercent -=
                (double)hintCount /
                (request.TrainingType == TrainingType.WritingFromNativeLanguage
                    ? wordEntity.TranslatedName.Length
                    : wordEntity.Name.Length) * LearningSettings.LearnStep;
        }

        newLevelPercent = Math.Max(newLevelPercent, 0);
        newLevelPercent = Math.Min(newLevelPercent, 100);

        if (isActive)
            await _wordRepository.UpdateActiveLearnLevel(request.WordId, newLevelPercent,
                cancellationToken);
        else
            await _wordRepository.UpdatePassiveLearnLevel(request.WordId, newLevelPercent,
                cancellationToken);

        await _wordChangeHistoryRepository.AddAsync(request.WordId, request.WasSuccessful,
            (int)vocabularyType, wordEntity.PassiveLearnedPercent, wordEntity.ActiveLearnedPercent,
            request.TrainingId, GetAnswerByTrainingType(wordEntity, request.TrainingType), request.Answer,
            cancellationToken);

        await UpdateXpLevel(user, request.WasSuccessful, cancellationToken);

        return true;
    }

    private async Task UpdateXpLevel(Domain.Entities.User user, bool isSuccess, CancellationToken token)
    {
        var requiredXp = CalculatorXP.CalculateXpRequired(user.Level);

        var newXp = user.XP + (isSuccess ? LearningSettings.SuccessXpStep : LearningSettings.FailXpStep);
        var newLevel = user.Level;

        if (newXp >= requiredXp)
        {
            newLevel++;
            newXp -= requiredXp;
        }

        await _usersRepository.UpdateXPLevel(newXp, newLevel, user.Id, token);
    }

    private string GetAnswerByTrainingType(WordDto word, TrainingType type)
    {
        if (TrainingTypesHelper.IsFromLearnTraining(type)) return word.TranslatedName;

        return word.Name;
    }
}