﻿using System.Security.Claims;
using LinguiCards.Application.Commands.Language.AddLanguageCommand;
using LinguiCards.Application.Commands.Language.DeleteLanguageCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.Language.GetAllLanguagesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/Language")]
[ApiController]
public class LanguageController : ControllerBase
{
    private readonly IMediator _mediator;

    public LanguageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("all")]
    [HttpGet]
    public async Task<List<LanguageDto>> GetAll()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetAllLanguagesQuery(username));
    }

    [HttpPost]
    public async Task<bool> Post(LanguageDto model)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new AddLanguageCommand(model, username));
    }

    [HttpDelete]
    public async Task<bool> Delete(int id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new DeleteLanguageCommand(id, username));
    }
}