using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildFlow.Documents.Infrastructure.Persistence.Configurations;

internal sealed class DocumentVersionConfiguration
    : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(new DocumentVersionIdConverter())
            .ValueGeneratedNever();

        // المفتاح الأجنبيّ نحو المستند، قويّ بمحوّله
        builder.Property(v => v.DocumentId)
            .HasConversion(new DocumentIdConverter())
            .IsRequired();

        builder.Property(v => v.VersionNumber).IsRequired();

        builder.Property(v => v.FileName)
            .IsRequired().HasMaxLength(300);

        builder.Property(v => v.FilePath)
            .IsRequired().HasMaxLength(1000);

        builder.Property(v => v.ContentType)
            .IsRequired().HasMaxLength(100);

        builder.Property(v => v.ChangeNotes).HasMaxLength(1000);

        // رقم الإصدار فريد ضمن المستند الواحد
        builder.HasIndex(v => new { v.DocumentId, v.VersionNumber })
            .IsUnique();
    }
}