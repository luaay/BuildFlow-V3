using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Enums;
using BuildFlow.Projects.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Projects.Infrastructure.Persistence.Repositories;

internal sealed class ProjectRepository(ProjectsDbContext context) : IProjectRepository
{
    public async Task AddAsync(Project project, CancellationToken ct = default)
    {
        await context.Projects.AddAsync(project, ct);
    }

    public async Task<Project?> GetByIdAsync(
        ProjectId id, Guid tenantId, CancellationToken ct = default)
    {
        // Include members so the aggregate loads complete.
        return await context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(
                p => p.Id == id && p.TenantId == tenantId, ct);
    }

    public async Task<Project?> GetByCodeAsync(
        string code, Guid tenantId, CancellationToken ct = default)
    {
        var upper = code.Trim().ToUpperInvariant();
        return await context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(
                p => p.TenantId == tenantId && p.Code.Value == upper, ct);
    }

    public async Task<bool> CodeExistsAsync(
        string code, Guid tenantId, CancellationToken ct = default)
    {
        var upper = code.Trim().ToUpperInvariant();
        return await context.Projects
            .AnyAsync(p => p.TenantId == tenantId && p.Code.Value == upper, ct);
    }

    public async Task<PagedResult<Project>> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        ProjectStatus? statusFilter,
        string? search,
        CancellationToken ct = default)
    {
        // Base query: this tenant's projects.
        var query = context.Projects
            .Include(p => p.Members)
            .Where(p => p.TenantId == tenantId);

        // Optional status filter.
        if (statusFilter.HasValue)
            query = query.Where(p => p.Status == statusFilter.Value);

        // Optional search on name or code.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) || p.Code.Value.Contains(term));
        }

        // Total before paging, to build PagedResult.
        var totalCount = await query.CountAsync(ct);

        // Page slice: Skip/Take in the database.
        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Project>(items, totalCount, page, pageSize);
    }

    public async Task<List<Project>> GetByMemberAsync(
        Guid userId, Guid tenantId, CancellationToken ct = default)
    {
        // Projects in this tenant where the user is a member.
        return await context.Projects
            .Include(p => p.Members)
            .Where(p => p.TenantId == tenantId
                     && p.Members.Any(m => m.UserId == userId))
            .ToListAsync(ct);
    }
}