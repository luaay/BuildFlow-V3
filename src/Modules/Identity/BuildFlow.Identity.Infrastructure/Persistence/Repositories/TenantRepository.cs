using BuildFlow.Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class TenantRepository(IdentityDbContext context) : ITenantRepository
{
    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .AnyAsync(t => t.Slug == slug, cancellationToken);
    }
}