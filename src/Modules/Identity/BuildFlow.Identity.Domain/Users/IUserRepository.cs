using BuildFlow.Identity.Domain.Tenants;

namespace BuildFlow.Identity.Domain.Users;

// عقد الوصول لبيانات المستخدم — التنفيذ في طبقة Infrastructure
public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        TenantId tenantId,
        Email email,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        TenantId tenantId,
        Email email,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<User> Users, int TotalCount)> GetPagedByTenantAsync(
        TenantId tenantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}