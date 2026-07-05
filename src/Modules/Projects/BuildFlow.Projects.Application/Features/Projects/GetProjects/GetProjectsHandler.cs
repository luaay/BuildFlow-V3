using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Application.Common;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Projects.GetProjects;

internal sealed class GetProjectsHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository)
    : IQueryHandler<GetProjectsQuery, PagedResult<ProjectDto>>
{
    public async Task<Result<PagedResult<ProjectDto>>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        // Tenant from the token; the repository already returns PagedResult.
        var paged = await projectRepository.GetPagedAsync(
            currentUser.TenantId,
            request.Page,
            request.PageSize,
            request.Status,
            request.Search,
            cancellationToken);

        // Map domain entities to DTOs, flattening Money into two fields.
        var dtos = paged.Items.Select(p => new ProjectDto(
            p.Id.Value,
            p.Name,
            p.Code.Value,
            p.Description,
            p.Status.ToString(),
            p.Budget.Amount,
            p.Budget.Currency,
            p.ClientName,
            p.Location,
            p.StartDate,
            p.EndDate,
            p.Members.Count,
            p.CreatedAtUtc)).ToList();

        var result = new PagedResult<ProjectDto>(
            dtos, paged.TotalCount, request.Page, request.PageSize);

        return Result.Ok(result);
    }
}