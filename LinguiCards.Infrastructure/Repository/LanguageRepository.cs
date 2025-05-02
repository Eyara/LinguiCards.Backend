using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure.Repository;

public class LanguageRepository : ILanguageRepository
{
    private readonly LinguiCardsDbContext _dbContext;
    private readonly IMapper _mapper;

    public LanguageRepository(LinguiCardsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<LanguageDto>> GetAllAsync(int userId, CancellationToken token)
    {
        return await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .Where(l => l.UserId == userId)
            .ProjectTo<LanguageDto>(_mapper.ConfigurationProvider)
            .ToListAsync(token);
    }

    public async Task<LanguageDto> GetByIdAsync(int languageId, CancellationToken token)
    {
        var languageEntity = await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .FirstOrDefaultAsync(l => l.Id == languageId, token);

        if (languageEntity == null) return null;

        return _mapper.Map<LanguageDto>(languageEntity);
    }

    public async Task<LanguageDto> GetByNameAndUserAsync(string name, int userId, CancellationToken token)
    {
        var languageEntity = await _dbContext.Languages
            .Include(l => l.LanguageDictionary)
            .FirstOrDefaultAsync(l => l.Name == name && l.UserId == userId, token);

        if (languageEntity == null) return null;

        return _mapper.Map<LanguageDto>(languageEntity);
    }

    public async Task AddAsync(LanguageAddDto language, int userId, CancellationToken token)
    {
        var languageEntity = new Language
        {
            Name = language.Name.ToLower().Trim().Replace('ё', 'е'),
            UserId = userId,
            LanguageDictionaryId = language.LanguageDictionaryId
        };

        await _dbContext.Languages.AddAsync(languageEntity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int languageId, CancellationToken token)
    {
        var entity = await _dbContext.Languages
            .FirstOrDefaultAsync(l => l.Id == languageId, token);

        _dbContext.Languages.Remove(entity);
        await _dbContext.SaveChangesAsync(token);
    }
}