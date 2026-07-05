using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Common;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Application.Features.Projects.GetProjects;

// Tenant comes from the token, not the query. Pagination has defaults.
public sealed record GetProjectsQuery(
    int Page = 1,
    int PageSize = 20,
    ProjectStatus? Status = null,
    string? Search = null) : IQuery<PagedResult<ProjectDto>>;