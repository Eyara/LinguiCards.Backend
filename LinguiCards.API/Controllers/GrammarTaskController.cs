using System.Security.Claims;
using LinguiCards.Application.Commands.GrammarTask.EvaluateGrammarTaskCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.GrammarTask.GetGrammarTaskQuery;
using LinguiCards.Application.Queries.GrammarTaskType.GetAllGrammarTaskTypeDictionariesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GrammarTaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public GrammarTaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<string> GetGrammarTask(int languageId, string level, string? topic = "", string? type = "")
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetGrammarTaskQuery(username, languageId, level, topic, type));
    }

    [Route("evaluation")]
    [HttpPost]
    public async Task<string> EvaluateGrammarTask(int languageId, string level, string taskText,
        string userAnswer, string? topic = "", string? type = "")
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new EvaluateGrammarTaskCommand(username, languageId, level, taskText, userAnswer, topic, type));
    }

    [Route("types")]
    [HttpGet]
    public async Task<List<GrammarTaskTypeDictionaryDto>> GetGrammarTaskTypes()
    {
        return await _mediator.Send(new GetAllGrammarTaskTypeDictionariesQuery());
    }
}

