using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface ILanguageRepository
{
    Task<List<LanguageDto>> GetAllAsync(int userId, CancellationToken token);
    Task<LanguageDto> GetByIdAsync(int languageId, CancellationToken token);
    Task AddAsync(LanguageDto language, int userId, CancellationToken token);
    Task DeleteAsync(int languageId, CancellationToken token);
}