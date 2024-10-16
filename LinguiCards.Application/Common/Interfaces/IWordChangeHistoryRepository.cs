﻿namespace LinguiCards.Application.Common.Interfaces;

public interface IWordChangeHistoryRepository
{
    Task AddAsync(int wordId, bool isCorrectAnswer, int type, double passiveLearned, double activeLearned,
        Guid? trainingId, string? answer, CancellationToken token);
}