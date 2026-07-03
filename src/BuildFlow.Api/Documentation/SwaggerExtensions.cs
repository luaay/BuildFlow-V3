using Microsoft.OpenApi.Models;

namespace BuildFlow.Api.Documentation;

internal static class SwaggerExtensions
{
    public static IServiceCollection AddBuildFlowSwagger(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Declare a Bearer (JWT) security scheme so Swagger shows an
            // Authorize button and sends the token on protected endpoints.
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste only the token. Swagger adds the "
                            + "'Bearer ' prefix automatically.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", scheme);

            // Require the scheme globally; anonymous endpoints still work.
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [scheme] = Array.Empty<string>()
            });
        });

        return services;
    }
}