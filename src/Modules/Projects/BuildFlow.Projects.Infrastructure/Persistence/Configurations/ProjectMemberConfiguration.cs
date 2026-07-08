using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Projects.Infrastructure.Persistence.Configurations;

internal sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");
        builder.HasKey(m => m.Id);

        // strongly-typed ID
        builder.Property(m => m.Id)
            .HasConversion(new ProjectMemberIdConverter())
            .ValueGeneratedNever();

        // strong ProjectId (internal reference within the module)
        builder.Property(m => m.ProjectId)
            .HasConversion(new ProjectIdConverter())
            .IsRequired();

        // raw Guid user reference on the boundary toward Identity
        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.JoinedAtUtc)
            .IsRequired();

        // a user appears at most once per project
        builder.HasIndex(m => new { m.ProjectId, m.UserId })
            .IsUnique();
    }
}