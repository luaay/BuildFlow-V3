using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildFlow.Identity.Infrastructure.Authentication;

internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim("sub", user.Id.Value.ToString()),
            new Claim("tenant", user.TenantId.Value.ToString()),
            new Claim("email", user.Email.Value),
            new Claim("role", user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}