using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Domain.Repositories;

public interface IProjectRepository
{
    // Strong ProjectId internally; raw Guid tenant on the boundary.
    Task<Project?> GetByIdAsync(ProjectId id, Guid tenantId, CancellationToken ct = default);

    Task<Project?> GetByCodeAsync(string code, Guid tenantId, CancellationToken ct = default);

    Task<bool> CodeExistsAsync(string code, Guid tenantId, CancellationToken ct = default);

    // Paged listing uses the shared PagedResult for consistency with Identity.
    Task<PagedResult<Project>> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        ProjectStatus? statusFilter,
        string? search,
        CancellationToken ct = default);

    Task AddAsync(Project project, CancellationToken ct = default);

    // Raw Guid user reference on the boundary toward Identity.
    Task<List<Project>> GetByMemberAsync(Guid userId, Guid tenantId, CancellationToken ct = default);
}