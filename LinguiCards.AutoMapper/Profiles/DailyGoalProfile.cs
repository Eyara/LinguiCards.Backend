using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class DailyGoalProfile : Profile
{
    public DailyGoalProfile()
    {
        CreateMap<DailyGoal, DailyGoalDTO>().ReverseMap();
    }
}
