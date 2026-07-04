namespace BuildFlow.SharedKernel.Domain.Auditing;

// عقد التدقيق — أي entity تطبّقه تُملأ حقوله تلقائياً في الـ Infrastructure
public interface IAuditableEntity
{
// IAuditableEntity — filled by the auditing interceptor in Infrastructure.
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
}