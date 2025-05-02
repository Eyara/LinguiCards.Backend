using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class UserSettingProfile : Profile
{
    public UserSettingProfile()
    {
        CreateMap<UserSetting, UserSettingDto>().ReverseMap();
    }
}
