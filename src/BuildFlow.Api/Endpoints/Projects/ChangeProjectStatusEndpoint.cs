using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Projects.ChangeProjectStatus;
using BuildFlow.Projects.Domain.Enums;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

internal static class ChangeProjectStatusEndpoint
{
    public static void MapChangeProjectStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/projects/{projectId:guid}/status", HandleAsync)
            .RequireAuthorization()
            .WithName("ChangeProjectStatus")
            .WithTags("Projects");
    }

    // الحالة الهدف تصل رقماً في الجسم
    public sealed record ChangeStatusRequest(ProjectStatus TargetStatus);

    private static async Task<IResult> HandleAsync(
        Guid projectId,
        ChangeStatusRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeProjectStatusCommand(projectId, request.TargetStatus);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}