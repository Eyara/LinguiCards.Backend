using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Common.Interfaces;

public interface IIrregularVerbRepository
{
    Task<IrregularVerbDto> GetByIdAsync(int irregularVerbId, CancellationToken token);
    Task<List<IrregularVerbDto>> GetByLanguageAsync(int languageId, CancellationToken token);
}

