﻿using LinguiCards.Application.Common.Models;
using MediatR;

namespace LinguiCards.Application.Queries.Word.GetAllWordsPaginatedQuery;

public record GetAllWordsPaginatedQuery
(int LanguageId, string Username, int PageNumber, int PageSize,
    string NameFilterQuery = "", string TranslationNameFilterQuery = "") : IRequest<PaginatedResult<WordDto>>;