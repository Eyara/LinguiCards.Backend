using MediatR;

namespace LinguiCards.Application.Commands.GrammarTask.EvaluateGrammarTaskCommand;

public record EvaluateGrammarTaskCommand(string Username, int LanguageId, string Level, string TaskText, string UserAnswer, string? Topic, string? Type) : IRequest<string>;

