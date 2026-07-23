using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Documents.Application.Features.Documents.CreateDocument;

internal sealed class CreateDocumentHandler(
    ICurrentUserService currentUser,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDocumentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var result = Document.Create(
            currentUser.TenantId,
            command.ProjectId,
            command.Title,
            command.Description,
            command.Type,
            command.FileName,
            command.FilePath,
            command.FileSizeBytes,
            command.ContentType,
            currentUser.UserId);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var document = result.Value;

        await documentRepository.AddAsync(document, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(document.Id.Value);
    }
}