using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class LanguageProfile : Profile
{
    public LanguageProfile()
    {
        CreateMap<Language, LanguageDto>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.LanguageDictionary.Url));

        CreateMap<LanguageAddDto, Language>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.LanguageDictionary, opt => opt.Ignore())
            .ForMember(dest => dest.Words, opt => opt.Ignore())
            .AfterMap((src,
                    dest) =>
                {
                    dest.Name = src.Name.ToLower().Trim().Replace('ё', 'е');
                }
            );
    }
}