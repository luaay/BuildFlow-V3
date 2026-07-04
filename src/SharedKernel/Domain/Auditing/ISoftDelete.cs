namespace BuildFlow.SharedKernel.Domain.Auditing;

// عقد الحذف الناعم — السجلّ يُعلَّم كمحذوف بدل محوه فعلياً
public interface ISoftDelete
{
    // ISoftDelete — the record is flagged, not physically removed.
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }
}