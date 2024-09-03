using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Commands.Language.AddLanguageCommand;

public record AddLanguageCommand(LanguageAddDto Language, string Username) : IRequest<bool>;