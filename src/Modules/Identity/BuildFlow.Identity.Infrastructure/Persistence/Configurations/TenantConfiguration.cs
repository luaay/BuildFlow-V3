using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        // تطبيق Value Converter للـ strongly-typed ID
        builder.Property(t => t.Id)
            .HasConversion(new TenantIdConverter())
            .ValueGeneratedNever(); // الـ Id يُولّد في الـ domain، لا في قاعدة البيانات

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        // الـ slug فريد (لا يتكرّر) — index فريد
        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>(); // الـ enum يُخزّن كـ integer

        builder.Property(t => t.Plan)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.SuspendedAtUtc);

        // حقول الـ auditing
        builder.Property(t => t.CreatedAtUtc).IsRequired();
        builder.Property(t => t.CreatedBy).IsRequired();
        builder.Property(t => t.ModifiedAtUtc);
        builder.Property(t => t.ModifiedBy);

        // حقول الـ soft delete
        builder.Property(t => t.IsDeleted).IsRequired();
        builder.Property(t => t.DeletedAtUtc);
        builder.Property(t => t.DeletedBy);

        // تجاهل الـ domain events — ليست عموداً في قاعدة البيانات
        builder.Ignore(t => t.DomainEvents);

        // Global Query Filter — استبعاد المحذوف ناعماً تلقائياً
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}