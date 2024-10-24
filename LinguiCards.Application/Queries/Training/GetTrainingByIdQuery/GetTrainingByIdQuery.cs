using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Training.GetTrainingByIdQuery;

public record GetTrainingByIdQuery(Guid TrainingId) : IRequest<List<WordChangeHistoryDTO>>;