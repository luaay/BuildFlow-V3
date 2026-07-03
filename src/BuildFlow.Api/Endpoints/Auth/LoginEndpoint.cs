using BuildFlow.Identity.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BuildFlow.Api.Errors;

namespace BuildFlow.Api.Endpoints.Auth;

// One endpoint class per vertical slice (REPR pattern).
// The class is a thin wrapper: receive, dispatch, translate.
internal static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", HandleAsync)
            // Login must stay open: the user has no token yet.
            .AllowAnonymous()
            .WithName("Login")
            .WithTags("Auth");
    }

    // The request body shape the frontend sends. Enums, if any, arrive
    // as integers; none here, all three fields are strings.
    public sealed record LoginRequest(string Slug, string Email, string Password);

    private static async Task<IResult> HandleAsync(
        [FromBody] LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Slug,
            request.Email,
            request.Password);

       var result = await sender.Send(command, cancellationToken);

        // Central translation: failures become RFC 7807 ProblemDetails.
        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
        return Results.Ok(result.Value);
    }
}