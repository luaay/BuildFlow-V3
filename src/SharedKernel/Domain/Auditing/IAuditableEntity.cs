namespace BuildFlow.SharedKernel.Domain.Auditing;

// عقد التدقيق — أي entity تطبّقه تُملأ حقوله تلقائياً في الـ Infrastructure
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
    Guid CreatedBy { get; set; }
    DateTime? ModifiedAtUtc { get; set; }
    Guid? ModifiedBy { get; set; }
}