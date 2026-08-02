using BuildFlow.SharedKernel.Domain.Auditing;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.SharedInfrastructure.Auditing;

// تنفيذ مستودع التدقيق Repository implementation
// يحقّق العقد IAuditRepository مستعملاً سياق التدقيق
public sealed class AuditRepository(AuditDbContext context) : IAuditRepository
{
    // إضافة عدّة سجلّات دفعةً، ثم الحفظ
    public async Task AddRangeAsync(
        IEnumerable<AuditEntry> entries,
        CancellationToken ct = default)
    {
        await context.AuditEntries.AddRangeAsync(entries, ct);
        await context.SaveChangesAsync(ct);
    }

    // جلب سجلّات كيان بعينه، الأحدث أوّلاً
    public async Task<List<AuditEntry>> GetByEntityAsync(
        string entityName,
        string entityId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        return await context.AuditEntries
            .Where(e => e.TenantId == tenantId
                     && e.EntityName == entityName
                     && e.EntityId == entityId)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(ct);
    }

    // جلب سجلّات المستأجر، مرقّمة، الأحدث أوّلاً
    public async Task<List<AuditEntry>> GetByTenantAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        return await context.AuditEntries
            .Where(e => e.TenantId == tenantId)
            .OrderByDescending(e => e.OccurredAt)
            .Skip((page - 1) * pageSize)   // نتخطّى الصفحات السابقة
            .Take(pageSize)                // نأخذ صفحةً واحدة
            .ToListAsync(ct);
    }

    // عدّ سجلّات المستأجر، للترقيم
    public async Task<int> CountByTenantAsync(
        Guid tenantId,
        CancellationToken ct = default)
    {
        return await context.AuditEntries
            .CountAsync(e => e.TenantId == tenantId, ct);
    }
}