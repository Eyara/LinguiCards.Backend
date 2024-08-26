using MediatR;

namespace LinguiCards.Application.Commands.Word.DeleteWordCommand;

public record DeleteWordCommand(int Id, string Username) : IRequest<bool>;