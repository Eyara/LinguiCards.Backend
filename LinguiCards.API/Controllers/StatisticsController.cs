using System.Security.Claims;
using LinguiCards.Application.Common.Models.Statistics;
using LinguiCards.Application.Queries.Statistics.GetAccuracyByTrainingTypeQuery;
using LinguiCards.Application.Queries.Statistics.GetActivityHeatmapQuery;
using LinguiCards.Application.Queries.Statistics.GetLearningVelocityQuery;
using LinguiCards.Application.Queries.Statistics.GetRetentionCurveQuery;
using LinguiCards.Application.Queries.Statistics.GetWeakestWordsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{languageId}/learning-velocity")]
    public async Task<LearningVelocityResponse> GetLearningVelocity(
        int languageId, [FromQuery] int weeks = 12)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetLearningVelocityQuery(username, languageId, weeks));
    }

    [HttpGet("{languageId}/retention-curve")]
    public async Task<RetentionCurveResponse> GetRetentionCurve(
        int languageId, [FromQuery] int rangeDays = 90, [FromQuery] int periodDays = 7)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetRetentionCurveQuery(username, languageId, rangeDays, periodDays));
    }

    [HttpGet("activity-heatmap")]
    public async Task<ActivityHeatmapResponse> GetActivityHeatmap([FromQuery] int? year = null)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetActivityHeatmapQuery(username, year));
    }

    [HttpGet("{languageId}/accuracy-by-training-type")]
    public async Task<AccuracyByTrainingTypeResponse> GetAccuracyByTrainingType(
        int languageId, [FromQuery] int? rangeDays = null)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetAccuracyByTrainingTypeQuery(username, languageId, rangeDays));
    }

    [HttpGet("{languageId}/weakest-words")]
    public async Task<WeakestWordsResponse> GetWeakestWords(
        int languageId, [FromQuery] int count = 20, [FromQuery] int minAttempts = 3)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetWeakestWordsQuery(username, languageId, count, minAttempts));
    }
}
