using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Errors;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.SubmitForReview;

internal sealed class SubmitForReviewHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<SubmitForReviewCommand>
{
    public async Task<Result> Handle(
        SubmitForReviewCommand command,
        CancellationToken cancellationToken)
    {
        var documentId = new DocumentId(command.DocumentId);

        var document = await documentRepository.GetByIdAsync(
            documentId, currentUser.TenantId, cancellationToken);

        if (document is null)
            return Result.Fail(DocumentErrors.NotFound(command.DocumentId));

        // منشئ المستند وحده يقدّمه للمراجعة
        if (document.CreatedBy != currentUser.UserId)
            return Result.Fail(DocumentErrors.Forbidden);

        var result = document.SubmitForReview(
            command.ReviewerId, currentUser.UserId);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}