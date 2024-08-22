using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguiCards.Controllers;

[Authorize]
[Route("api/Languages")]
[ApiController]
public class LanguageController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        throw new NotImplementedException();
    }
}