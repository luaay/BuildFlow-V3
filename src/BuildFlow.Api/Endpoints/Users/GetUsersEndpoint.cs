using BuildFlow.Api.Errors;
using BuildFlow.Identity.Application.Features.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Users;

// One endpoint class per vertical slice (REPR pattern).
// Protected: requires a valid token. The tenant is never passed in;
// the handler reads it from ICurrentUserService via the token.
internal static class GetUsersEndpoint
{
    public static void MapGetUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", HandleAsync)
            // Requires authentication. A request without a valid token
            // is rejected with 401 before reaching the handler.
            .RequireAuthorization()
            .WithName("GetUsers")
            .WithTags("Users");
    }

    // Pagination comes from the query string. Both have defaults in the
    // query record, so the client may call /api/users with no parameters.
    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetUsersQuery(page, pageSize);

        var result = await sender.Send(query, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
    }
}