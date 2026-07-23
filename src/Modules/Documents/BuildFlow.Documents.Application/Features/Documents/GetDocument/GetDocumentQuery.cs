using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Common;

namespace BuildFlow.Documents.Application.Features.Documents.GetDocument;

public sealed record GetDocumentQuery(Guid DocumentId)
    : IQuery<DocumentDetailDto>;