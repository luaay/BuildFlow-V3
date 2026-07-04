using BuildFlow.SharedKernel.Domain;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Domain.Events;

public record ProjectMemberAddedEvent(
    Guid ProjectId,
    Guid TenantId,
    Guid UserId,
    ProjectMemberRole Role) : DomainEvent;
