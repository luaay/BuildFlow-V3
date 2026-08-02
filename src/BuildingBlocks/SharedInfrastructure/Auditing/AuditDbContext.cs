using BuildFlow.SharedKernel.Domain.Auditing;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.SharedInfrastructure.Auditing;

// سياق قاعدة بيانات التدقيق DbContext، جدول واحد لسجلّات التدقيق
public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options)
    : DbContext(options)
{
    // مجموعة سجلّات التدقيق DbSet
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // إعداد جدول سجلّ التدقيق
        modelBuilder.Entity<AuditEntry>(entity =>
        {
            // entity.ToTable("AuditEntries");
            entity.HasKey(e => e.Id);

            // اسم الكيان مطلوب، بطول محدود
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(200);

            // القيم كـ JSON، قد تطول، فنجعلها نصّاً بلا حدّ
            entity.Property(e => e.OldValues);
            entity.Property(e => e.NewValues);
            entity.Property(e => e.ChangedColumns);

            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            // فهرسة index على المستأجر والوقت، لتسريع الجلب
            entity.HasIndex(e => new { e.TenantId, e.OccurredAt });

            // فهرسة على الكيان، لجلب سجلّات كيان بعينه
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
        });
    }
}