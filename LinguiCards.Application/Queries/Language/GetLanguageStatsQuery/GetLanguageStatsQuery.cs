using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Language.GetLanguageStatsQuery;

public record GetLanguageStatsQuery(string Username, int LanguageId) : IRequest<LanguageDashboardStat>;