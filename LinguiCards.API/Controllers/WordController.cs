using System.Security.Claims;
using LinguiCards.Application.Commands.Word.AddWordCommand;
using LinguiCards.Application.Commands.Word.AddWordListCommand;
using LinguiCards.Application.Commands.Word.DeleteWordCommand;
using LinguiCards.Application.Commands.Word.UpdateLearnLevelCommand;
using LinguiCards.Application.Commands.Word.UpdateWordCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Word.GetAllWordsPaginatedQuery;
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

    [Route("/api/Language/{languageId}/Word")]
    [HttpGet]
    public async Task<PaginatedResult<WordDto>> GetAllPaginated(int languageId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 15)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetAllWordsPaginatedQuery(languageId, username, pageNumber, pageSize));
    }

    [Route("unlearned")]
    [HttpGet]
    public async Task<List<TrainingWord>> GetUnlearned(int languageId)
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

    [Route("/api/Language/{languageId}/Words")]
    [HttpPost]
    public async Task<bool> Post(IEnumerable<WordDto> words, int languageId)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new AddWordListCommand(words, languageId, username));
    }

    [HttpPut]
    public async Task<bool> Put(int wordId, string name, string translationName)
    {
        return await _mediator.Send(new UpdateWordCommand(wordId, name, translationName));
    }

    [Route("updateLearnLevel")]
    [HttpPatch]
    public async Task<bool> UpdateLearnLevel(int wordId, TrainingType trainingType, bool wasSuccessful,
        Guid? trainingId, string? answer)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new UpdateLearnLevelCommand(wordId, trainingType, wasSuccessful, trainingId, answer,
            username));
    }

    [HttpDelete]
    public async Task<bool> Delete(int id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new DeleteWordCommand(id, username));
    }
}