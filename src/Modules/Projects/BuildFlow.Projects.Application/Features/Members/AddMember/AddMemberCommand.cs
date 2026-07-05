using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Application.Features.Members.AddMember;

// Tenant and requester come from the token. Role arrives as an integer.
public sealed record AddMemberCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectMemberRole Role) : ICommand;