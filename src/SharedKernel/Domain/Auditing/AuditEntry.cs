namespace BuildFlow.SharedKernel.Domain.Auditing;

// سجلّ ثابت لتغيير واحد على كيان Immutable audit entry
// سجلّ واحد لكل كيان في كل عملية حفظ SaveChanges
public class AuditEntry
{
    public Guid     Id           { get; init; } = Guid.NewGuid();
    public Guid     TenantId     { get; init; }
    public Guid?    UserId       { get; init; }           // من أجرى التغيير the actor
    public string   EntityName   { get; init; } = null!;  // اسم الكيان entity name
    public string   EntityId     { get; init; } = null!;  // المفتاح الأساسيّ primary key نصّاً
    public AuditAction Action    { get; init; }           // العملية Created/Updated/Deleted
    public string?  OldValues    { get; init; }           // لقطة JSON قبل التغيير
    public string?  NewValues    { get; init; }           // لقطة JSON بعد التغيير
    public string?  ChangedColumns { get; init; }         // الأعمدة المتغيّرة changed columns
    public DateTime OccurredAt   { get; init; } = DateTime.UtcNow;
    public string?  IpAddress    { get; init; }
    public string?  UserAgent    { get; init; }
}