using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTextForTranslationQuery;

public record GetTextForTranslationQuery(string Username, int LanguageId, int Length, string Level, string? Topic) : IRequest<string>;