using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        TenantId tenantId,
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(
                u => u.TenantId == tenantId && u.Email == email,
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        TenantId tenantId,
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(
                u => u.TenantId == tenantId && u.Email == email,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Users, int TotalCount)> GetPagedByTenantAsync(
        TenantId tenantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // الاستعلام الأساس: مستخدمو هذا المستأجر
        var query = context.Users
            .Where(u => u.TenantId == tenantId);

        // العدد الكلي (قبل التصفّح) — لحساب PagedResult
        var totalCount = await query.CountAsync(cancellationToken);

        // التصفّح الفعلي: Skip/Take في قاعدة البيانات
        var users = await query
            .OrderBy(u => u.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}