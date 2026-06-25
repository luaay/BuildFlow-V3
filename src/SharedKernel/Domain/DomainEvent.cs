namespace BuildFlow.SharedKernel.Domain;

// base record لكل الأحداث — يوفّر EventId و OccurredOnUtc تلقائياً
// كل حدث مشتق يكتفي بإعلان بياناته الخاصة
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}