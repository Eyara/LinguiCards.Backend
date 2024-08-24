using MediatR;
using Microsoft.Extensions.Configuration;

namespace LinguiCards.Application.Queries.User.GetUserTokenQuery;

public record GetUserTokenQuery(IConfiguration Configuration, string Username, string Password) : IRequest<string>
{
}