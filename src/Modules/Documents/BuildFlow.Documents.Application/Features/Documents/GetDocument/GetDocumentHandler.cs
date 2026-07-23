using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Application.Common;
using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Errors;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.GetDocument;

internal sealed class GetDocumentHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository)
    : IQueryHandler<GetDocumentQuery, DocumentDetailDto>
{
    public async Task<Result<DocumentDetailDto>> Handle(
        GetDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var documentId = new DocumentId(request.DocumentId);

        var document = await documentRepository.GetByIdAsync(
            documentId, currentUser.TenantId, cancellationToken);

        if (document is null)
            return Result.Fail(DocumentErrors.NotFound(request.DocumentId));

        var dto = new DocumentDetailDto(
            document.Id.Value,
            document.ProjectId,
            document.Title,
            document.Description,
            document.Type.ToString(),
            document.Status.ToString(),
            document.CurrentVersionNumber,
            document.ReviewerId,
            document.SubmittedForReviewAtUtc,
            document.ReviewedAtUtc,
            document.ReviewNotes,
            document.Versions
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new DocumentVersionDto(
                    v.Id.Value,
                    v.VersionNumber,
                    v.FileName,
                    v.FileSizeBytes,
                    v.ContentType,
                    v.ChangeNotes,
                    v.UploadedBy,
                    v.UploadedAtUtc))
                .ToList(),
            document.CreatedAtUtc,
            document.ModifiedAtUtc);

        return Result.Ok(dto);
    }
}