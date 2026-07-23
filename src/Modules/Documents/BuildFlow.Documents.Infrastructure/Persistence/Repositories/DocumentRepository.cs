using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Enums;
using BuildFlow.Documents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Documents.Infrastructure.Persistence.Repositories;

internal sealed class DocumentRepository(DocumentsDbContext context)
    : IDocumentRepository
{
    public async Task AddAsync(
        Document document, CancellationToken cancellationToken = default)
    {
        await context.Documents.AddAsync(document, cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(
        DocumentId id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.Documents
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(
                d => d.Id == id && d.TenantId == tenantId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Document> Documents, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? projectId = null,
        DocumentStatus? status = null,
        DocumentType? type = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Documents
            .Where(d => d.TenantId == tenantId);

        if (projectId.HasValue)
            query = query.Where(d => d.ProjectId == projectId.Value);

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        if (type.HasValue)
            query = query.Where(d => d.Type == type.Value);

        // البحث على العنوان، عمود نصّيّ مباشر بلا محوّل
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(d => d.Title.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var documents = await query
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (documents, totalCount);
    }
}