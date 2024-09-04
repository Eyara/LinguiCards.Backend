using System.Security.Claims;
using LinguiCards.Application.Commands.Word.AddWordCommand;
using LinguiCards.Application.Commands.Word.DeleteWordCommand;
using LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Word.GetUnlearnedWordsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WordController : ControllerBase
{
    private readonly IMediator _mediator;

    public WordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("unlearned")]
    [HttpGet]
    public async Task<List<WordDto>> GetAll(int languageId)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetUnlearnedWordsQuery(languageId, username));
    }

    [Route("/api/Language/{languageId}/Word")]
    [HttpPost]
    public async Task<bool> Post(WordDto model, int languageId)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new AddWordCommand(model, languageId, username));
    }

    [Route("updateLearnLevel")]
    [HttpPut]
    public async Task<bool> Put(int wordId, bool wasSuccessful)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new UpdateLearnLevelCommand(wordId, wasSuccessful, username));
    }


    [HttpDelete]
    public async Task<bool> Delete(int id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new DeleteWordCommand(id, username));
    }
}