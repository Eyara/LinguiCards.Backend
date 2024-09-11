using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetAllWordsQuery;

public record GetAllWordsQuery(int LanguageId, string Username) : IRequest<List<WordDto>>;