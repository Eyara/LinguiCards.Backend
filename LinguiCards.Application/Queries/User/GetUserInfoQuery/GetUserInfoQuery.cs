using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.User.GetUserInfoQuery;

public record GetUserInfoQuery(string Username) : IRequest<UserInfo>
{
}