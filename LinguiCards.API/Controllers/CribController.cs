using System.Security.Claims;
using LinguiCards.Application.Commands.Language.AddLanguageCommand;
using LinguiCards.Application.Commands.Language.DeleteLanguageCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Crib.GetAllCribsQuery;
using LinguiCards.Application.Queries.Language.GetAllLanguagesQuery;
using LinguiCards.Application.Queries.LanguageDictionary.GetAllLanguageDictionariesQuery;
using LinguiCards.Application.Queries.LanguageDictionary.GetAvailableLanguagesQuery;
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

    [Route("all")]
    [HttpGet]
    public async Task<List<CribDTO>> GetAll(int languageId)
    {
        return await _mediator.Send(new GetAllCribsQuery(languageId));
    }
}