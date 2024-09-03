using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface ILanguageDictionaryRepository
{
    Task<List<LanguageDictionaryDto>> GetAllAsync(CancellationToken token);
}