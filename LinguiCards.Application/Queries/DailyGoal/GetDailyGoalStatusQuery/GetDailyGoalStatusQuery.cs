using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.DailyGoal.GetDailyGoalStatusQuery;

public record GetDailyGoalStatusQuery(string Username) : IRequest<DailyGoalStatusDto>;
