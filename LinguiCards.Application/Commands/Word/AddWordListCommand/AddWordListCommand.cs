using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Commands.Word.AddWordCommand;

public record AddWordListCommand(IEnumerable<WordDto> Words, int LanguageId, string Username) : IRequest<bool>;