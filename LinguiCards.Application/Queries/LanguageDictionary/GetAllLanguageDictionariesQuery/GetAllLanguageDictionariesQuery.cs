using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.LanguageDictionary.GetAllLanguageDictionariesQuery;

public record GetAllLanguageDictionariesQuery : IRequest<List<LanguageDictionaryDto>>;