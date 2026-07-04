using BuildFlow.SharedKernel.Domain;
using BuildFlow.Projects.Domain.Enums;

namespace BuildFlow.Projects.Domain.Entities;

public sealed class ProjectMember : Entity<ProjectMemberId>
{
    public ProjectId ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public ProjectMemberRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    private ProjectMember() : base() { }

    private ProjectMember(ProjectMemberId id) : base(id) { }

    internal static ProjectMember Create(
        ProjectId projectId, Guid userId, ProjectMemberRole role) =>
        new(ProjectMemberId.New())
        {
            ProjectId   = projectId,
            UserId      = userId,
            Role        = role,
            JoinedAtUtc = DateTime.UtcNow
        };

    internal void ChangeRole(ProjectMemberRole newRole) => Role = newRole;
}