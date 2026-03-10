using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetLearningVelocityQuery;

public record GetLearningVelocityQuery(string Username, int LanguageId, int Weeks = 12)
    : IRequest<LearningVelocityResponse>;
