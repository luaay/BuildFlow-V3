using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Projects.CreateProject;
using MediatR;

namespace BuildFlow.Api.Endpoints.Projects;

// One endpoint class per vertical slice (REPR pattern).
// Protected: the tenant and creator are read from the token, never sent in.
internal static class CreateProjectEndpoint
{
    public static void MapCreateProjectEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects", HandleAsync)
            .RequireAuthorization()
            .WithName("CreateProject")
            .WithTags("Projects");
    }

    private static async Task<IResult> HandleAsync(
        CreateProjectCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            // 201 Created with the new project's id.
            : Results.Created($"/api/projects/{result.Value}", new { id = result.Value });
    }
}