using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class WordChangeHistoryRepository : IWordChangeHistoryRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public WordChangeHistoryRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddAsync(
        int wordId,
        bool isCorrectAnswer,
        int vocabularyType,
        double passiveLearned,
        double activeLearned,
        Guid? trainingId,
        string? correctAnswer,
        string? answer,
        CancellationToken token)
    {
        var history = new WordChangeHistory
        {
            WordId = wordId,
            IsCorrectAnswer = isCorrectAnswer,
            VocabularyType = vocabularyType,
            PassiveLearned = passiveLearned,
            ActiveLearned = activeLearned,
            TrainingId = trainingId,
            CorrectAnswer = correctAnswer,
            Answer = answer,
            ChangedOn = DateTime.UtcNow
        };

        await _dbContext.WordChangeHistories.AddAsync(history, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<List<WordChangeHistoryDTO>> GetAllByIdAsync(Guid trainingId, CancellationToken token)
    {
        return await _dbContext.WordChangeHistories
            .Where(h => h.TrainingId == trainingId)
            .ProjectTo<WordChangeHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }

    public async Task<List<WordChangeHistoryDTO>> GetByLanguageIdAsync(int languageId, DateTime? from,
        CancellationToken token)
    {
        var query = _dbContext.WordChangeHistories
            .AsNoTracking()
            .Where(h => h.Word.LanguageId == languageId);

        if (from.HasValue)
            query = query.Where(h => h.ChangedOn >= from.Value);

        return await query
            .OrderBy(h => h.ChangedOn)
            .ProjectTo<WordChangeHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }

    public async Task<Dictionary<int, List<WordChangeHistoryDTO>>> GetGroupedByWordAsync(int languageId,
        int? minAttempts, CancellationToken token)
    {
        var query = _dbContext.WordChangeHistories
            .AsNoTracking()
            .Where(h => h.Word.LanguageId == languageId);

        var grouped = await query
            .GroupBy(h => h.WordId)
            .Where(g => !minAttempts.HasValue || g.Count() >= minAttempts.Value)
            .Select(g => new
            {
                WordId = g.Key,
                Histories = g.Select(h => new WordChangeHistoryDTO
                {
                    Id = h.Id,
                    IsCorrectAnswer = h.IsCorrectAnswer,
                    PassiveLearned = h.PassiveLearned,
                    ActiveLearned = h.ActiveLearned,
                    TrainingId = h.TrainingId,
                    WordId = h.WordId,
                    VocabularyType = h.VocabularyType,
                    ChangedOn = h.ChangedOn,
                    Answer = h.Answer,
                    CorrectAnswer = h.CorrectAnswer
                }).ToList()
            })
            .ToListAsync(token);

        return grouped.ToDictionary(g => g.WordId, g => g.Histories);
    }
}