using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class LanguageDictionaryProfile : Profile
{
    public LanguageDictionaryProfile()
    {
        CreateMap<LanguageDictionary, LanguageDictionaryDto>().ReverseMap();
    }
}
