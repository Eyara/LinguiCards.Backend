using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinguiCards.Application.Common.Interfaces;
using LinguiCards.Application.Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LinguiCards.Application.Queries.User.GetUserTokenQuery;

public class GetUserTokenQueryHandler : IRequestHandler<GetUserTokenQuery, string>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserTokenQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<string> Handle(GetUserTokenQuery request, CancellationToken cancellationToken)
    {
        if (await IsValidUser(request.Username, request.Password, cancellationToken))
            return GenerateJwtToken(request.Username, request.Configuration);

        // TODO: add a middleware to handle custom Exceptions
        throw new Exception();
    }

    private async Task<bool> IsValidUser(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByNameAsync(username, cancellationToken);

        if (user == null) return false;

        return PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.Salt);
    }

    private string GenerateJwtToken(string username, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}