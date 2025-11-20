using AutoMapper;
using LinguiCards.Application.Common.Models;
using LinguiCards.Domain.Entities;

namespace LinguiCards.AutoMapper.Profiles;

public class IrregularVerbProfile : Profile
{
    public IrregularVerbProfile()
    {
        CreateMap<IrregularVerb, IrregularVerbDto>()
            .ReverseMap();
    }
}

