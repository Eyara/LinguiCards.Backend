using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.UserSetting.GetUserSettingQuery;

public record GetUserSettingQuery(string Username) : IRequest<UserSettingDto?>
{
}