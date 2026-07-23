using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Application.Common;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.GetDocuments;

internal sealed class GetDocumentsHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository)
    : IQueryHandler<GetDocumentsQuery, PagedResult<DocumentSummaryDto>>
{
    public async Task<Result<PagedResult<DocumentSummaryDto>>> Handle(
        GetDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        var (documents, totalCount) = await documentRepository.GetPagedAsync(
            currentUser.TenantId,
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Status,
            request.Type,
            request.Search,
            cancellationToken);

        var items = documents
            .Select(d => new DocumentSummaryDto(
                d.Id.Value,
                d.ProjectId,
                d.Title,
                d.Type.ToString(),
                d.Status.ToString(),
                d.CurrentVersionNumber,
                d.CreatedAtUtc))
            .ToList();

        var result = new PagedResult<DocumentSummaryDto>(
            items, totalCount, request.Page, request.PageSize);

        return Result.Ok(result);
    }
}