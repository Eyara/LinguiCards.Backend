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
    private readonly IDailyGoalRepository _dailyGoalRepository;
    private readonly IUserSettingRepository _userSettingRepository;

    public UpdateLearnLevelCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository, IWordChangeHistoryRepository wordChangeHistoryRepository,
        IDailyGoalRepository dailyGoalRepository, IUserSettingRepository userSettingRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
        _wordChangeHistoryRepository = wordChangeHistoryRepository;
        _dailyGoalRepository = dailyGoalRepository;
        _userSettingRepository = userSettingRepository;
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

        var answerLength = isActive
            ? (request.TrainingType == TrainingType.WritingFromNativeLanguage
                ? wordEntity.Name.Length
                : wordEntity.TranslatedName.Length)
            : 0;

        var hintCount = request.HintCount ?? 0;
        var quality = SrsCalculator.MapQuality(request.WasSuccessful, hintCount, answerLength);

        var currentEf = isActive ? wordEntity.ActiveEaseFactor : wordEntity.PassiveEaseFactor;
        var currentInterval = isActive ? wordEntity.ActiveIntervalDays : wordEntity.PassiveIntervalDays;
        var currentReps = isActive ? wordEntity.ActiveRepetitionCount : wordEntity.PassiveRepetitionCount;

        if (currentEf < SrsCalculator.MinEaseFactor)
            currentEf = SrsCalculator.DefaultEaseFactor;

        var srs = SrsCalculator.Calculate(currentEf, currentInterval, currentReps, quality);
        var nextReview = DateTime.UtcNow.Date.AddDays(srs.IntervalDays);
        var learnedPercent = SrsCalculator.DeriveLearnedPercent(srs.RepetitionCount, srs.IntervalDays);

        await _wordRepository.UpdateSrsState(request.WordId, vocabularyType, srs.EaseFactor,
            srs.IntervalDays, srs.RepetitionCount, nextReview, learnedPercent, cancellationToken);

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

        var xpGained = isSuccess ? LearningSettings.SuccessXpStep : LearningSettings.FailXpStep;
        var newXp = user.XP + xpGained;
        var newLevel = user.Level;

        if (newXp >= requiredXp)
        {
            newLevel++;
            newXp -= requiredXp;
        }

        await _usersRepository.UpdateXPLevel(newXp, newLevel, user.Id, token);

        var userSettings = await _userSettingRepository.GetByUserIdAsync(user.Id, token);
        var targetXp = userSettings?.DailyGoalXp ?? 0;

        await _dailyGoalRepository.AddXpAsync(user.Id, xpGained, targetXp, token);
    }

    private string GetAnswerByTrainingType(WordDto word, TrainingType type)
    {
        if (TrainingTypesHelper.IsFromLearnTraining(type)) return word.TranslatedName;

        return word.Name;
    }
}