using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IDefaultCribRepository
{
    Task<List<CribDTO>> GetAllAsync(int languageId, CancellationToken token);
}