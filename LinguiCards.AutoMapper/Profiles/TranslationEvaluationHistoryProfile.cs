using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class TranslationEvaluationHistoryProfile: Profile
{
    public TranslationEvaluationHistoryProfile()
    {
        CreateMap<TranslationEvaluationHistory, TranslationEvaluationHistoryDTO>()
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}