using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Errors;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.ApproveDocument;

internal sealed class ApproveDocumentHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ApproveDocumentCommand>
{
    public async Task<Result> Handle(
        ApproveDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var documentId = new DocumentId(command.DocumentId);

        var document = await documentRepository.GetByIdAsync(
            documentId, currentUser.TenantId, cancellationToken);

        if (document is null)
            return Result.Fail(DocumentErrors.NotFound(command.DocumentId));

        // حراسة هوية المراجع داخل الكيان نفسه
        var result = document.Approve(currentUser.UserId, command.Notes);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}