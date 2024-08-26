using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Commands.Word.AddWordCommand;

public record AddWordCommand(WordDto Word, int LanguageId, string Username) : IRequest<bool>;