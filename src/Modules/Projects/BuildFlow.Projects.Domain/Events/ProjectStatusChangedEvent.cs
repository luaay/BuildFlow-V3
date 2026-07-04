using BuildFlow.SharedKernel.Domain;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Domain.Events;

public record ProjectStatusChangedEvent(
    Guid ProjectId,
    Guid TenantId,
    ProjectStatus NewStatus) : DomainEvent;
