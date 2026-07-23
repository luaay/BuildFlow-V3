using BuildFlow.Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Documents.Infrastructure.Persistence;

public sealed class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options)
    : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // طبّق كل إعدادات الكيانات في هذه التجميعة تلقائياً
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DocumentsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}