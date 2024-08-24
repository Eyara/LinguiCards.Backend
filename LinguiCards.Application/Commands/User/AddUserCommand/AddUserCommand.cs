using LinguiCards.Application.Common;
using MediatR;

namespace LinguiCards.Application.Commands.User.AddUserCommand;

public record AddUserCommand(string Username, string Password) : IRequest<RequestResult>;