using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Identity.Domain.Users.Events;

// يُطلق عند إنشاء مستخدم جديد
public sealed record UserCreatedEvent(UserId UserId, TenantId TenantId, string Email)
    : DomainEvent;