using BuildFlow.Projects.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildFlow.Projects.Application.EventHandlers;

// Reacts to a new project being created.
internal sealed class ProjectCreatedEventHandler(
    ILogger<ProjectCreatedEventHandler> logger)
    : INotificationHandler<ProjectCreatedEvent>
{
    public Task Handle(
        ProjectCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        // For now: logging only. Later: notify members, seed folders, etc.
        logger.LogInformation(
            "Project created: {ProjectId} ({ProjectName}) in tenant {TenantId} at {OccurredOn}",
            notification.ProjectId,
            notification.ProjectName,
            notification.TenantId,
            notification.OccurredOnUtc);

        return Task.CompletedTask;
    }
}