using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.DailyGoal.GetDailyGoalStatusQuery;
using LinguiCards.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Route("api/Notification")]
[ApiController]
[ApiKeyAuth]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("daily")]
    public async Task<DailyGoalStatusDto> GetDailyGoalStatus([FromQuery] string username)
    {
        return await _mediator.Send(new GetDailyGoalStatusQuery(username));
    }
}
