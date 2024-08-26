using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;

public record UpdateLearnLevelCommand(int WordId, bool WasSuccessful, string Username) : IRequest<bool>;