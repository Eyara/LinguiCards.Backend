using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetRetentionCurveQuery;

public record GetRetentionCurveQuery(string Username, int LanguageId, int RangeDays = 90, int PeriodDays = 7)
    : IRequest<RetentionCurveResponse>;
