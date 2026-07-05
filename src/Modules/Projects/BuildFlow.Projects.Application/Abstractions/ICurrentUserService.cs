namespace BuildFlow.Projects.Application.Abstractions;

public interface ICurrentUserService
{
    // Raw Guids: these cross the boundary from the Identity module.
    Guid UserId { get; }
    Guid TenantId { get; }
}