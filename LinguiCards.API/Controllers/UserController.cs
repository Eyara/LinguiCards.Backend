using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinguiCards.Controllers.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LinguiCards.Controllers;

[Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginModel login)
    {
        if (IsValidUser(login))
        {
            var token = GenerateJwtToken(login.Username);
            return Ok(new { token });
        }

        return Unauthorized();
    }

    private bool IsValidUser(UserLoginModel login)
    {
        // Replace with your user validation logic
        return login.Username == "testuser" && login.Password == "password";
    }

    private string GenerateJwtToken(string username)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}