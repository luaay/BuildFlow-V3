using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Application.Common;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Projects.GetProject;

internal sealed class GetProjectHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository)
    : IQueryHandler<GetProjectQuery, ProjectDetailDto>
{
    public async Task<Result<ProjectDetailDto>> Handle(
        GetProjectQuery request,
        CancellationToken cancellationToken)
    {
        // Wrap the raw Guid from the request into the strong ProjectId.
        var projectId = new ProjectId(request.ProjectId);

        var project = await projectRepository.GetByIdAsync(
            projectId, currentUser.TenantId, cancellationToken);

        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(projectId));

        var dto = new ProjectDetailDto(
            project.Id.Value,
            project.Name,
            project.Code.Value,
            project.Description,
            project.Status.ToString(),
            project.Budget.Amount,
            project.Budget.Currency,
            project.ClientName,
            project.Location,
            project.StartDate,
            project.EndDate,
            project.Members
                .Select(m => new ProjectMemberDto(
                    m.UserId, m.Role.ToString(), m.JoinedAtUtc))
                .ToList(),
            project.CreatedAtUtc,
            project.ModifiedAtUtc);

        return Result.Ok(dto);
    }
}