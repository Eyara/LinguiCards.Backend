using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Crib.GetAllCribsQuery;

public record GetAllCribsQuery(int LanguageId) : IRequest<List<CribDTO>>;