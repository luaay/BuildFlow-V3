using BuildFlow.Application.Abstractions;

namespace BuildFlow.Projects.Application.Features.Projects.UpdateProject;

// Tenant and editor come from the token, not the request.
public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    string? ClientName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal Budget,
    string Currency) : ICommand;