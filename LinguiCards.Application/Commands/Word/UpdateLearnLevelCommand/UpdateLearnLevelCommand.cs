using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;

public record UpdateLearnLevelCommand(int WordId, TrainingType TrainingType, bool WasSuccessful, Guid? TrainingId,
    string? Answer, string Username) : IRequest<bool>;