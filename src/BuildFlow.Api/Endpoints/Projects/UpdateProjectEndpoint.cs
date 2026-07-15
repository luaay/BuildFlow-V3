using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Projects.UpdateProject;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

internal static class UpdateProjectEndpoint
{
    public static void MapUpdateProjectEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/projects/{projectId:guid}", HandleAsync)
            .RequireAuthorization()
            .WithName("UpdateProject")
            .WithTags("Projects");
    }

    // المعرّف من المسار، وبقيّة الحقول من الجسم
    public sealed record UpdateProjectRequest(
        string Name,
        string? Description,
        string? ClientName,
        string? Location,
        DateTime? StartDate,
        DateTime? EndDate,
        decimal Budget,
        string Currency);

    private static async Task<IResult> HandleAsync(
        Guid projectId,
        UpdateProjectRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(
            projectId,
            request.Name,
            request.Description,
            request.ClientName,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.Budget,
            request.Currency);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}