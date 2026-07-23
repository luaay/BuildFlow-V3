using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Errors;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.AddVersion;

internal sealed class AddVersionHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddVersionCommand>
{
    public async Task<Result> Handle(
        AddVersionCommand command,
        CancellationToken cancellationToken)
    {
        var documentId = new DocumentId(command.DocumentId);

        var document = await documentRepository.GetByIdAsync(
            documentId, currentUser.TenantId, cancellationToken);

        if (document is null)
            return Result.Fail(DocumentErrors.NotFound(command.DocumentId));

        // الكيان يحرس: لا إصدار قيد المراجعة ولا بعد الاعتماد
        var result = document.AddVersion(
            command.FileName,
            command.FilePath,
            command.FileSizeBytes,
            command.ContentType,
            command.ChangeNotes,
            currentUser.UserId);

        if (result.IsFailed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}