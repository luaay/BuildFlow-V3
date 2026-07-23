using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Common;
using BuildFlow.Documents.Domain.Enums;

namespace BuildFlow.Documents.Application.Features.Documents.GetDocuments;

public sealed record GetDocumentsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ProjectId = null,
    DocumentStatus? Status = null,
    DocumentType? Type = null,
    string? Search = null) : IQuery<PagedResult<DocumentSummaryDto>>;