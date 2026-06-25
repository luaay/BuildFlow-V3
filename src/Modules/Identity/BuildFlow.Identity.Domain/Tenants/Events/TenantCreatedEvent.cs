using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Identity.Domain.Tenants.Events;

// يُطلق عند إنشاء مستأجر جديد
// يرث EventId و OccurredOnUtc تلقائياً من DomainEvent
public sealed record TenantCreatedEvent(TenantId TenantId, string Name) : DomainEvent;