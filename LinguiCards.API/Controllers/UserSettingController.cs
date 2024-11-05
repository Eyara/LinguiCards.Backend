using System.Security.Claims;
using LinguiCards.Application.Commands.UserSetting.AddOrUpdateUserSettingCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.UserSetting.GetUserSettingQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserSettingController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserSettingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<UserSettingDto?> Get()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetUserSettingQuery(username));
    }
    
    [HttpPost]
    public async Task Post(int activeTrainingSize, int passiveTrainingSize)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        await _mediator.Send(new AddOrUpdateUserSettingCommand(username, activeTrainingSize, passiveTrainingSize));
    }
}