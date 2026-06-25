using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Identity.Domain.Tenants.Events;

// يُطلق عند إيقاف المستأجر مؤقتاً
public sealed record TenantSuspendedEvent(TenantId TenantId) : DomainEvent;