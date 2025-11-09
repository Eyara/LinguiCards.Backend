using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class GrammarTaskTypeDictionaryProfile : Profile
{
    public GrammarTaskTypeDictionaryProfile()
    {
        CreateMap<GrammarTaskTypeDictionary, GrammarTaskTypeDictionaryDto>().ReverseMap();
    }
}

