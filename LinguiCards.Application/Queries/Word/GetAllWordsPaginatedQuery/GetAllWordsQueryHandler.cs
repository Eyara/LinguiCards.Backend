using LinguiCards.Application.Common.Exceptions;
using LinguiCards.Application.Common.Exceptions.Base;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetAllWordsPaginatedQuery;

public class GetAllWordsPaginatedQueryHandler : IRequestHandler<GetAllWordsPaginatedQuery, PaginatedResult<WordDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IWordRepository _wordRepository;

    public GetAllWordsPaginatedQueryHandler(ILanguageRepository languageRepository, IUsersRepository usersRepository,
        IWordRepository wordRepository)
    {
        _languageRepository = languageRepository;
        _usersRepository = usersRepository;
        _wordRepository = wordRepository;
    }

    public async Task<PaginatedResult<WordDto>> Handle(GetAllWordsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(request.Username, cancellationToken);

        if (user == null) throw new UserNotFoundException();

        var languageEntity = await _languageRepository.GetByIdAsync(request.LanguageId, cancellationToken);

        if (languageEntity == null) throw new LanguageNotFoundException();

        if (languageEntity.UserId != user.Id) throw new EntityOwnershipException();

        return await _wordRepository.GetAllPaginatedAsync(request.LanguageId, request.PageNumber, request.PageSize);
    }
}