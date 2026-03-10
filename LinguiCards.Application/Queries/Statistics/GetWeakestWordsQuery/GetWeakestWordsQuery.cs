using LinguiCards.Application.Common.Models.Statistics;
using MediatR;

namespace LinguiCards.Application.Queries.Statistics.GetWeakestWordsQuery;

public record GetWeakestWordsQuery(string Username, int LanguageId, int Count = 20, int MinAttempts = 3)
    : IRequest<WeakestWordsResponse>;
