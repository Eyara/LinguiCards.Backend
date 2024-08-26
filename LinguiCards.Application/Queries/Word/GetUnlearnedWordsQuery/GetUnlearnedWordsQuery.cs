using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetUnlearnedWordsQuery;

public record GetUnlearnedWordsQuery(int LanguageId, string Username) : IRequest<List<WordDto>>;