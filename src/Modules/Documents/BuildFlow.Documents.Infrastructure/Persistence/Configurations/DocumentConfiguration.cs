using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Documents.Infrastructure.Persistence.Configurations;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        // المفتاح القويّ بمحوّله، ولا يُولَّد
        builder.Property(d => d.Id)
            .HasConversion(new DocumentIdConverter())
            .ValueGeneratedNever();

        // مراجع خام على الحدود، بلا محوّل
        builder.Property(d => d.TenantId).IsRequired();
        builder.Property(d => d.ProjectId).IsRequired();

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.ReviewNotes)
            .HasMaxLength(2000);

        // العلاقة مع الإصدارات: حذف متتالٍ، فالإصدار لا يعيش بلا مستنده
        builder.HasMany(d => d.Versions)
            .WithOne()
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // فهرس للبحث بالمستأجر والمشروع، أكثر استعلام شيوعاً
        builder.HasIndex(d => new { d.TenantId, d.ProjectId });

        // تجاهل أحداث المجال، فهي ليست بيانات مخزّنة
        builder.Ignore(d => d.DomainEvents);

        // تصفية عامّة: أخفِ المحذوف ناعماً
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}