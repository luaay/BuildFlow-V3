using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.ValueObjects;
using BuildFlow.Projects.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Projects.Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);

        // strongly-typed ID
        builder.Property(p => p.Id)
            .HasConversion(new ProjectIdConverter())
            .ValueGeneratedNever();

        // raw Guid tenant reference on the boundary toward Identity
        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // ProjectCode value object — stored as its string value
        builder.Property(p => p.Code)
            .HasConversion(
                code => code.Value,                  // عند الحفظ
                value => ProjectCode.Create(value))  // عند القراءة
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        // Money value object — mapped as an owned type (two columns)
        builder.OwnsOne(p => p.Budget, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BudgetAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BudgetCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(p => p.ClientName)
            .HasMaxLength(200);

        builder.Property(p => p.Location)
            .HasMaxLength(200);

        builder.Property(p => p.StartDate);
        builder.Property(p => p.EndDate);

        // project code is unique within a tenant
        builder.HasIndex(p => new { p.TenantId, p.Code })
            .IsUnique();

        // auditing
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.CreatedBy).IsRequired();
        builder.Property(p => p.ModifiedAtUtc);
        builder.Property(p => p.ModifiedBy);

        // soft delete
        builder.Property(p => p.IsDeleted).IsRequired();
        builder.Property(p => p.DeletedAtUtc);
        builder.Property(p => p.DeletedBy);

        // members relationship: a project owns its member list
        builder.HasMany(p => p.Members)
            .WithOne()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.DomainEvents);
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}