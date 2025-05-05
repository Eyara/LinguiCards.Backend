using LinguiCards.Application.Common.Models.Integration;
using MediatR;

namespace LinguiCards.Application.Queries.TranslationEvaluation.GetTranslationEvaluationQuery;

public record GetTranslationEvaluationQuery(string Level, string OriginalText, string Translation) : IRequest<TranslationEvaluationDTO>;