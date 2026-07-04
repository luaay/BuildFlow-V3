using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Projects.Domain.Events;

public record ProjectCreatedEvent(
    Guid ProjectId,
    Guid TenantId,
    string ProjectName,
    Guid CreatedByUserId) : DomainEvent;
