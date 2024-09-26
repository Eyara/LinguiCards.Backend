using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.LanguageDictionary.GetAvailableLanguagesQuery;

public record GetAvailableLanguagesQuery(string Username) : IRequest<List<LanguageDictionaryDto>>;