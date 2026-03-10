using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetActivityHeatmapQuery;

public record GetActivityHeatmapQuery(string Username, int? Year = null)
    : IRequest<ActivityHeatmapResponse>;
