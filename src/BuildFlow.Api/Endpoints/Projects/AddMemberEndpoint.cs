using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Members.AddMember;
using BuildFlow.Projects.Domain.Enums;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

internal static class AddMemberEndpoint
{
    public static void MapAddMemberEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId:guid}/members", HandleAsync)
            .RequireAuthorization()
            .WithName("AddMember")
            .WithTags("Projects");
    }

    // المعرّف من المسار، والعضو ودوره من الجسم
    public sealed record AddMemberRequest(Guid UserId, ProjectMemberRole Role);

    private static async Task<IResult> HandleAsync(
        Guid projectId,
        AddMemberRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddMemberCommand(projectId, request.UserId, request.Role);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}