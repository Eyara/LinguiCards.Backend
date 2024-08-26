﻿using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Constants;
using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;

public class UpdateLearnLevelCommandHandler : IRequestHandler<UpdateLearnLevelCommand, bool>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public UpdateLearnLevelCommandHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<bool> Handle(UpdateLearnLevelCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new Exception();

        var wordEntity = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (wordEntity == null) throw new Exception();

        var languageEntity = await _languageRepository.GetByIdAsync(wordEntity.LanguageId, cancellationToken);

        if (languageEntity == null) throw new Exception();

        if (languageEntity.UserId != user.Id) throw new Exception();

        var newLevelPercent = request.WasSuccessful
            ? wordEntity.LearnedPercent + LearningSettings.LearnStep
            : wordEntity.LearnedPercent - LearningSettings.LearnStep;


        await _wordRepository.UpdateLearnLevel(request.WordId, newLevelPercent, cancellationToken);
        return true;
    }
}