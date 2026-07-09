using BuildFlow.Api.Errors;
using BuildFlow.Projects.Application.Features.Projects.GetProjects;
using BuildFlow.Projects.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Projects;

// Protected list endpoint. The tenant is read from the token; the
// handler filters by it, so cross-tenant listing is impossible.
internal static class GetProjectsEndpoint
{
    public static void MapGetProjectsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects", HandleAsync)
            .RequireAuthorization()
            .WithName("GetProjects")
            .WithTags("Projects");
    }

    // Pagination, status filter, and search come from the query string.
    // All have defaults, so /api/projects works with no parameters.
    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] ProjectStatus? status = null,
        [FromQuery] string? search = null)
    {
        var query = new GetProjectsQuery(page, pageSize, status, search);
        var result = await sender.Send(query, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
    }
}