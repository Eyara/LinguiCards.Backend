using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Commands.Word.ChallengeAnswerCommand;

public record ChallengeAnswerCommand(int WordId, Guid TrainingId, string Username) : IRequest<bool>;