using MediatR;

namespace BuildFlow.SharedKernel.Domain;

// عقد لكل "شيء مهم حدث" في النطاق — بصيغة الماضي: UserCreated, DocumentApproved
public interface IDomainEvent : INotification
{
    Guid EventId { get; }            // لدعم الـ Idempotency
    DateTime OccurredOnUtc { get; }  // توقيت الحدوث بـ UTC
}