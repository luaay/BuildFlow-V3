using BuildFlow.Projects.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildFlow.Projects.Infrastructure.Persistence.Converters;

// محوّل ProjectId ↔ Guid
public sealed class ProjectIdConverter : ValueConverter<ProjectId, Guid>
{
    public ProjectIdConverter()
        : base(
            id => id.Value,                 // عند الحفظ: ProjectId → Guid
            value => new ProjectId(value))  // عند القراءة: Guid → ProjectId
    {
    }
}

// محوّل ProjectMemberId ↔ Guid
public sealed class ProjectMemberIdConverter : ValueConverter<ProjectMemberId, Guid>
{
    public ProjectMemberIdConverter()
        : base(
            id => id.Value,                       // عند الحفظ: ProjectMemberId → Guid
            value => new ProjectMemberId(value))  // عند القراءة: Guid → ProjectMemberId
    {
    }
}