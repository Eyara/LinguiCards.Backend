using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetAccuracyByTrainingTypeQuery;

public record GetAccuracyByTrainingTypeQuery(string Username, int LanguageId, int? RangeDays = null)
    : IRequest<AccuracyByTrainingTypeResponse>;
