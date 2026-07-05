using BuildFlow.Application.Abstractions;

namespace BuildFlow.Projects.Application.Features.Projects.CreateProject;

// Tenant and creator come from ICurrentUserService, not the request.
public sealed record CreateProjectCommand(
    string Name,
    string Code,
    string? Description,
    decimal Budget,
    string Currency,
    string? ClientName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate) : ICommand<Guid>;