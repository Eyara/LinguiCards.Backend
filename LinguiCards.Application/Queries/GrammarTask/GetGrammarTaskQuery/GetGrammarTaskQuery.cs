using MediatR;

namespace LinguiCards.Application.Queries.GrammarTask.GetGrammarTaskQuery;

public record GetGrammarTaskQuery(string Username, int LanguageId, string Level, string? Topic, string? Type) : IRequest<string>;

