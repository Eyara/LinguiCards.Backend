using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Training.GetTrainingByIdQuery;

public class GetTrainingByIdQueryHandler : IRequestHandler<GetTrainingByIdQuery, List<WordChangeHistoryDTO>>
{
    private readonly IWordChangeHistoryRepository _historyRepository;

    public GetTrainingByIdQueryHandler(IWordChangeHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task<List<WordChangeHistoryDTO>> Handle(GetTrainingByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _historyRepository.GetAllByIdAsync(request.TrainingId, cancellationToken);
    }
}