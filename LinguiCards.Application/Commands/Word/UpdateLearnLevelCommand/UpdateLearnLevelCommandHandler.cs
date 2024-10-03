﻿using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Constants;
using LinguiCards.Application.Helpers;
using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;

public class UpdateLearnLevelCommandHandler : IRequestHandler<UpdateLearnLevelCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;
    private readonly IWordChangeHistoryRepository _wordChangeHistoryRepository;

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

        var newLevelPercent = request.WasSuccessful
            ? wordEntity.PassiveLearnedPercent + LearningSettings.LearnStep
            : wordEntity.PassiveLearnedPercent - LearningSettings.LearnStep;

        await _wordRepository.UpdatePassiveLearnLevel(request.WordId, newLevelPercent >= 0 ? newLevelPercent : 0,
            cancellationToken);

        await _wordChangeHistoryRepository.AddAsync(request.WordId, request.WasSuccessful, cancellationToken);
        
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
}