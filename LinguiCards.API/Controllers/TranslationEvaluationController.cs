using System.Security.Claims;
using LinguiCards.Application.Commands.Language.AddLanguageCommand;
using LinguiCards.Application.Commands.Language.DeleteLanguageCommand;
using LinguiCards.Application.Common.Models;
using LinguiCards.Application.Common.Models.Integration;
using LinguiCards.Application.Queries.Language.GetAllLanguagesQuery;
using LinguiCards.Application.Queries.Language.GetLanguageStatsQuery;
using LinguiCards.Application.Queries.LanguageDictionary.GetAllLanguageDictionariesQuery;
using LinguiCards.Application.Queries.LanguageDictionary.GetAvailableLanguagesQuery;
using LinguiCards.Application.Queries.TranslationEvaluation.GetTextForTranslationQuery;
using LinguiCards.Application.Queries.TranslationEvaluation.GetTranslationEvaluationQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TranslationEvaluationController : ControllerBase
{
    private readonly IMediator _mediator;

    public TranslationEvaluationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<string> GetTextForTranslation(int length, string level, string? topic = "")
    {
        return await _mediator.Send(new GetTextForTranslationQuery(length, level, topic));
    }
    
    [Route("evaluation")]
    [HttpGet]
    public async Task<TranslationEvaluationDTO> GetTranslationEvaluation(string level, string originalText, string translation)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return await _mediator.Send(new GetTranslationEvaluationQuery(username, level, originalText, translation));
    }
}