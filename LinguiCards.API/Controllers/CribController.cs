using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Crib.GetAllCribsQuery;
using LinguiCards.Application.Queries.IrregularVerb.GetIrregularVerbsByLanguageQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CribController : ControllerBase
{
    private readonly IMediator _mediator;

    public CribController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("{languageId:int}/cribs")]
    [HttpGet]
    public async Task<List<CribDTO>> GetAll(int languageId)
    {
        return await _mediator.Send(new GetAllCribsQuery(languageId));
    }

    [Route("{languageId:int}/irregular-verbs")]
    [HttpGet]
    public async Task<List<IrregularVerbDto>> GetIrregularVerbs(int languageId)
    {
        return await _mediator.Send(new GetIrregularVerbsByLanguageQuery(languageId));
    }
}