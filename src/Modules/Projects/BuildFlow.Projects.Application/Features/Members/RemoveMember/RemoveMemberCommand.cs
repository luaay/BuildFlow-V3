using BuildFlow.Application.Abstractions;

namespace BuildFlow.Projects.Application.Features.Members.RemoveMember;

// Tenant and requester come from the token.
public sealed record RemoveMemberCommand(
    Guid ProjectId,
    Guid UserId) : ICommand;