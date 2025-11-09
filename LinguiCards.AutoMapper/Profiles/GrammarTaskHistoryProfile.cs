using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class GrammarTaskHistoryProfile: Profile
{
    public GrammarTaskHistoryProfile()
    {
        CreateMap<GrammarTaskHistory, GrammarTaskHistoryDTO>()
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

