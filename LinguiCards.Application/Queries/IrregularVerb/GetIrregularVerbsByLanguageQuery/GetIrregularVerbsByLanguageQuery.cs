using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.IrregularVerb.GetIrregularVerbsByLanguageQuery;

public record GetIrregularVerbsByLanguageQuery(int LanguageId) : IRequest<List<IrregularVerbDto>>;

