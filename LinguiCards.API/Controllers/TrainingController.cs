using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Training.GetTrainingByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TrainingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrainingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("{trainingId}")]
    [HttpGet]
    public async Task<List<WordChangeHistoryDTO>> GetAll(Guid trainingId)
    {
        return await _mediator.Send(new GetTrainingByIdQuery(trainingId));
    }
}