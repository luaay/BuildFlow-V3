using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Members.RemoveMember;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

internal static class RemoveMemberEndpoint
{
    public static void MapRemoveMemberEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId:guid}/members/{userId:guid}", HandleAsync)
            .RequireAuthorization()
            .WithName("RemoveMember")
            .WithTags("Projects");
    }

    private static async Task<IResult> HandleAsync(
        Guid projectId,
        Guid userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMemberCommand(projectId, userId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}