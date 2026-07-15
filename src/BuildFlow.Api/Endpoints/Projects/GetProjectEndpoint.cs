using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Projects.GetProject;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

internal static class GetProjectEndpoint
{
    public static void MapGetProjectEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId:guid}", HandleAsync)
            .RequireAuthorization()
            .WithName("GetProject")
            .WithTags("Projects");
    }

    private static async Task<IResult> HandleAsync(
        Guid projectId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(projectId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
    }
}