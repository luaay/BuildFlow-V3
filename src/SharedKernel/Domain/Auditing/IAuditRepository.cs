namespace BuildFlow.SharedKernel.Domain.Auditing;

// عقد مستودع التدقيق Audit repository contract، تنفّذه البنية التحتية Infrastructure
public interface IAuditRepository
{
    // إضافة عدّة سجلّات دفعةً add a batch
    Task AddRangeAsync(IEnumerable<AuditEntry> entries, CancellationToken ct = default);

    // جلب سجلّات كيان بعينه by entity
    Task<List<AuditEntry>> GetByEntityAsync(
        string entityName,
        string entityId,
        Guid tenantId,
        CancellationToken ct = default);

    // جلب سجلّات المستأجر tenant، مرقّمة paged
    Task<List<AuditEntry>> GetByTenantAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    // عدّ سجلّات المستأجر count، للترقيم
    Task<int> CountByTenantAsync(Guid tenantId, CancellationToken ct = default);
}