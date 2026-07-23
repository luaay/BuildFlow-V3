using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Enums;

namespace BuildFlow.Documents.Domain.Repositories;

public interface IDocumentRepository
{
    Task AddAsync(Document document, CancellationToken cancellationToken = default);

    // الجلب مقيّد بالمستأجر دائماً، فالعزل بنيويّ
    Task<Document?> GetByIdAsync(
        DocumentId id,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    // قائمة مرقّمة، مع تصفية اختيارية بالمشروع والحالة والنوع
    Task<(IReadOnlyList<Document> Documents, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? projectId = null,
        DocumentStatus? status = null,
        DocumentType? type = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}