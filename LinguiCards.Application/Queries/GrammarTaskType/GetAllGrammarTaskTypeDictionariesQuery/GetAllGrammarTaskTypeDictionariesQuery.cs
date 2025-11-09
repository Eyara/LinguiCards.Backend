using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.GrammarTaskType.GetAllGrammarTaskTypeDictionariesQuery;

public record GetAllGrammarTaskTypeDictionariesQuery : IRequest<List<GrammarTaskTypeDictionaryDto>>;

