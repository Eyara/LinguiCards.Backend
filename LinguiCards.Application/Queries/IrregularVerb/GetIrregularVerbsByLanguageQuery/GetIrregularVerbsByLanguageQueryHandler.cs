using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.IrregularVerb.GetIrregularVerbsByLanguageQuery;

public class GetIrregularVerbsByLanguageQueryHandler : IRequestHandler<GetIrregularVerbsByLanguageQuery, List<IrregularVerbDto>>
{
    private readonly IIrregularVerbRepository _irregularVerbRepository;

    public GetIrregularVerbsByLanguageQueryHandler(IIrregularVerbRepository irregularVerbRepository)
    {
        _irregularVerbRepository = irregularVerbRepository;
    }

    public async Task<List<IrregularVerbDto>> Handle(GetIrregularVerbsByLanguageQuery request, CancellationToken cancellationToken)
    {
        return await _irregularVerbRepository.GetByLanguageAsync(request.LanguageId, cancellationToken);
    }
}

