using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BuildFlow.Identity.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildFlow.Api.Authentication;

internal static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Turn OFF the legacy claim-name remapping. Without this, the short
        // "sub" claim our JwtProvider writes is silently renamed to a long
        // URI on read, and CurrentUserService would find it empty.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // Bind the SAME options the JwtProvider signs with, so signing and
        // validation always share one source of truth.
        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Stop the modern handler from rewriting short claim names
                // like "sub" into long legacy URIs on read.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    // Tell the auth system which claim carries the name and
                    // role, so [Authorize] and User.Identity work as expected.
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        return services;
    }
}