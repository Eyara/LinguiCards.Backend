using MediatR;

namespace LinguiCards.Application.Commands.Language.DeleteLanguageCommand;

public record DeleteLanguageCommand(int Id, string Username) : IRequest<bool>;