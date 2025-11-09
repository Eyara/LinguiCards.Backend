using LinguiCards.Application.Common.Models.Integration;
using MediatR;

namespace LinguiCards.Application.Commands.GrammarTask.EvaluateGrammarTaskCommand;

public record EvaluateGrammarTaskCommand(string Username, string Level, string TaskText, string UserAnswer, string? Topic, string? Type) : IRequest<GrammarTaskEvaluationDTO>;

