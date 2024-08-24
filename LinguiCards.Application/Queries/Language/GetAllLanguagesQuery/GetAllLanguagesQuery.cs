using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Language.GetAllLanguagesQuery;

public record GetAllLanguagesQuery(string Username) : IRequest<List<LanguageDto>>;