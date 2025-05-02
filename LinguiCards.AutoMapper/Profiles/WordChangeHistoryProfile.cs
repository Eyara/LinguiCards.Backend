using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class WordChangeHistoryProfile : Profile
{
    public WordChangeHistoryProfile()
    {
        CreateMap<WordChangeHistory, WordChangeHistoryDTO>()
            .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.CorrectAnswer))
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer))
            .ForMember(dest => dest.ChangedOn, opt => opt.MapFrom(src => src.ChangedOn))
            .ReverseMap()
            .ForMember(dest => dest.Word, opt => opt.Ignore())
            .ForMember(dest => dest.TrainingId, opt => opt.Ignore());
    }
}