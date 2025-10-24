using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.DailyGoal.GetTodayGoalQuery;

public record GetTodayGoalQuery
    (string Username) : IRequest<DailyGoalDTO?>;