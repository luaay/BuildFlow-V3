using BuildFlow.SharedKernel.Domain.Auditing;

namespace BuildFlow.SharedInfrastructure.Auditing.Queries;

// كائن نقل DTO لعرض سجلّ التدقيق في الواجهة
public sealed record AuditEntryDto(
    Guid Id,
    Guid? UserId,
    string EntityName,
    string EntityId,
    string Action,            // نصّاq، لعرضه: Created/Updated/Deleted
    string? ChangedColumns,
    string? OldValues,
    string? NewValues,
    DateTime OccurredAt,
    string? IpAddress);

// طريقة تحويل من الكيان إلى كائن النقل mapping
public static class AuditEntryMapping
{
    public static AuditEntryDto ToDto(this AuditEntry entry) => new(
        entry.Id,
        entry.UserId,
        entry.EntityName,
        entry.EntityId,
        entry.Action.ToString(),   // نحوّل التعداd إلى نصّ
        entry.ChangedColumns,
        entry.OldValues,
        entry.NewValues,
        entry.OccurredAt,
        entry.IpAddress);
}