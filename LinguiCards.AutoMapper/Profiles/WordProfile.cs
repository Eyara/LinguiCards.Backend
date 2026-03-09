using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Helpers;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class WordProfile : Profile
{
    public WordProfile()
    {
        CreateMap<Word, WordDto>()
            .ReverseMap()
            .AfterMap((src, dest) =>
            {
                dest.Name = src.Name.ToLower().Trim().Replace('ё', 'е');
                dest.TranslatedName = src.TranslatedName.ToLower().Trim().Replace('ё', 'е');
                dest.CreatedOn = DateTime.UtcNow;
                dest.LastUpdated = DateTime.UtcNow;
                dest.PassiveLearnedPercent = 0;
                dest.ActiveLearnedPercent = 0;
                dest.PassiveEaseFactor = SrsCalculator.DefaultEaseFactor;
                dest.ActiveEaseFactor = SrsCalculator.DefaultEaseFactor;
                dest.PassiveIntervalDays = 0;
                dest.ActiveIntervalDays = 0;
                dest.PassiveRepetitionCount = 0;
                dest.ActiveRepetitionCount = 0;
                dest.PassiveNextReviewDate = null;
                dest.ActiveNextReviewDate = null;
            });

        CreateMap<Word, WordExtendedDTO>()
            .IncludeBase<Word, WordDto>()
            .ForMember(dest => dest.Histories, opt => opt.MapFrom(src => src.Histories))
            .ReverseMap()
            .ForMember(dest => dest.Histories, opt => opt.MapFrom(src => src.Histories));
    }
}