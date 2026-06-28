using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // strongly-typed ID
        builder.Property(u => u.Id)
            .HasConversion(new UserIdConverter())
            .ValueGeneratedNever();

        // العلاقة بالمستأجر بالـ ID — نحوّل TenantId أيضاً
        builder.Property(u => u.TenantId)
            .HasConversion(new TenantIdConverter())
            .IsRequired();

        // Email value object — نخزّن قيمته كعمود string
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,           // عند الحفظ: Email → string
                value => Email.Create(value).Value) // عند القراءة: string → Email
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.AccessFailedCount).IsRequired();
        builder.Property(u => u.LockoutEndUtc);

        // البريد فريد ضمن المستأجر الواحد (لا عالمياً)
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique();

        // auditing
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.CreatedBy).IsRequired();
        builder.Property(u => u.ModifiedAtUtc);
        builder.Property(u => u.ModifiedBy);

        // soft delete
        builder.Property(u => u.IsDeleted).IsRequired();
        builder.Property(u => u.DeletedAtUtc);
        builder.Property(u => u.DeletedBy);

        builder.Ignore(u => u.DomainEvents);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}