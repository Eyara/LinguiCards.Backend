using MediatR;

namespace LinguiCards.Application.Commands.DailyGoal.AddOrUpdateDailyGoalCommand;

public record AddOrUpdateDailyGoalCommand
    (string Username, int GainedXp) : IRequest;