namespace BuildFlow.Documents.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid TenantId { get; }
}