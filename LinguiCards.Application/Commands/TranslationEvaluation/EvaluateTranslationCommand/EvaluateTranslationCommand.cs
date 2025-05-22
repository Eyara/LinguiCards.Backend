using LinguiCards.Application.Common.Models.Integration;
using MediatR;

namespace LinguiCards.Application.Commands.TranslationEvaluation.EvaluateTranslationCommand;

public record EvaluateTranslationCommand(string Username, string Level, string OriginalText, string Translation) : IRequest<TranslationEvaluationDTO>;