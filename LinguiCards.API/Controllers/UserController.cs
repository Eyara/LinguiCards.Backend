using System.Security.Claims;
using LinguiCards.Application.Commands.User.AddUserCommand;
using LinguiCards.Application.Common;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Queries.User.GetUserInfoQuery;
using LinguiCards.Application.Queries.User.GetUserTokenQuery;
using LinguiCards.Controllers.Models.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public UserController(IConfiguration configuration, IMediator mediator)
    {
        _configuration = configuration;
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpGet("info")]
    public async Task<UserInfo> Info()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetUserInfoQuery(username));
    }

    [HttpPost("register")]
    public async Task<RequestResult> Register([FromBody] UserLoginModel model)
    {
        return await _mediator.Send(new AddUserCommand(model.Username, model.Password));
    }

    [HttpPost("login")]
    public async Task<string> Login([FromBody] UserLoginModel model)
    {
        return await _mediator.Send(new GetUserTokenQuery(_configuration, model.Username, model.Password));
    }
}