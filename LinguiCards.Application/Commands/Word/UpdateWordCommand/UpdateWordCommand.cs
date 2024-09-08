using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateWordCommand;

public record UpdateWordCommand(int WordId, string Name, string TranslationName) : IRequest<bool>;