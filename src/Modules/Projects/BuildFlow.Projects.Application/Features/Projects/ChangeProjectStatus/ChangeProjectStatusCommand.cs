using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Application.Features.Projects.ChangeProjectStatus;

// Tenant comes from the token. TargetStatus is sent as an integer.
public sealed record ChangeProjectStatusCommand(
    Guid ProjectId,
    ProjectStatus TargetStatus) : ICommand;