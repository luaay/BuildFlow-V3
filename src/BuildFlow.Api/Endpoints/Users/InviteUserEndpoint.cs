using BuildFlow.Api.Errors;
using BuildFlow.Identity.Application.Features.Users.InviteUser;
using BuildFlow.Identity.Domain.Users.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Users;

// One endpoint class per vertical slice (REPR pattern).
// Protected write: an authenticated user invites a new user into
// their own tenant. Tenant and inviter come from the token, not the body.
internal static class InviteUserEndpoint
{
    public static void MapInviteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/invite", HandleAsync)
            .RequireAuthorization()
            .WithName("InviteUser")
            .WithTags("Users");
    }

    // Role is an enum: the frontend sends it as an integer, and the
    // model binder maps it to UserRole. The body carries no tenant.
    public sealed record InviteUserRequest(
        string Email,
        string FullName,
        string InitialPassword,
        UserRole Role);

    private static async Task<IResult> HandleAsync(
        [FromBody] InviteUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new InviteUserCommand(
            request.Email,
            request.FullName,
            request.InitialPassword,
            request.Role);

        var result = await sender.Send(command, cancellationToken);

        // On success the handler returns the new user's id. Use 201 Created
        // to signal a new resource was created.
        return result.IsFailed
            ? result.ToProblem()
            : Results.Created($"/api/users/{result.Value}", new { id = result.Value });
    }
}