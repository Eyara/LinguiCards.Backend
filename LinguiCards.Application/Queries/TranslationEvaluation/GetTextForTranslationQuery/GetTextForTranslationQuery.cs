using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTextForTranslationQuery;

public record GetTextForTranslationQuery(string Username, int languageId, int Length, string Level, string? Topic) : IRequest<string>;