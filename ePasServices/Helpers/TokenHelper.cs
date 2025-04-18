using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class TokenHelper
{
    public static string GenerateAccessToken(string username, string app, byte[] key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim("username", username),
            new Claim("app", app)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        );

        return tokenHandler.WriteToken(token);
    }

    public static string GenerateRefreshToken(string username, byte[] key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim("username", username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(90),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        );

        return tokenHandler.WriteToken(token);
    }

    public static Dictionary<string, string>? DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.ToDictionary(c => c.Type, c => c.Value);
    }

    public static string GenerateResetToken(string username, string key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var token = new JwtSecurityToken(
            claims: new[] { new Claim("username", username), new Claim("purpose", "reset-password") },
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        );

        return tokenHandler.WriteToken(token);
    }

}