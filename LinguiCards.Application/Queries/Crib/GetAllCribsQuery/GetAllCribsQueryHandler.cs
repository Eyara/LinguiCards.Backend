using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Crib.GetAllCribsQuery;

public class GetAllCribsQueryHandler : IRequestHandler<GetAllCribsQuery, List<CribDTO>>
{
    private readonly IDefaultCribRepository _defaultCribRepository;

    public GetAllCribsQueryHandler(IDefaultCribRepository defaultCribRepository)
    {
        _defaultCribRepository = defaultCribRepository;
    }

    public async Task<List<CribDTO>> Handle(GetAllCribsQuery request, CancellationToken cancellationToken)
    {
        return await _defaultCribRepository.GetAllAsync(request.LanguageId, cancellationToken);
    }
}