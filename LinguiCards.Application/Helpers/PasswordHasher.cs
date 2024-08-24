using System.Security.Cryptography;
using System.Text;

namespace LinguiCards.Application.Helpers;

public static class PasswordHasher
{
    public static void CreatePasswordHash(string password, out string passwordHash, out string salt)
    {
        using var hmac = new HMACSHA512();
        salt = Convert.ToBase64String(hmac.Key);
        passwordHash =
            Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }
}