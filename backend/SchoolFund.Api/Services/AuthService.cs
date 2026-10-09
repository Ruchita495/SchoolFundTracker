using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SchoolFund.Api.Models;

namespace SchoolFund.Api.Services;

public class AuthService(IConfiguration config)
{
    private readonly PasswordHasher<AppUser> _hasher = new();
    public static SymmetricSecurityKey Key(IConfiguration c) => new(Encoding.UTF8.GetBytes(c["Jwt:Key"]!));

    public string Hash(AppUser u, string password) => _hasher.HashPassword(u, password);
    public bool Verify(AppUser u, string password) =>
        _hasher.VerifyHashedPassword(u, u.PasswordHash, password) != PasswordVerificationResult.Failed;

    public string CreateToken(AppUser u)
    {
        var claims = new[] { new Claim(ClaimTypes.Name, u.Username), new Claim(ClaimTypes.Role, u.Role) };
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(Key(config), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
