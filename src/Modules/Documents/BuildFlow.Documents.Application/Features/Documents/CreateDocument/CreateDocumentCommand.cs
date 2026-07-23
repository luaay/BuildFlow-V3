using BuildFlow.Application.Abstractions;
using BuildFlow.Documents.Domain.Enums;

namespace BuildFlow.Documents.Application.Features.Documents.CreateDocument;

// المستأجر والمنشئ من الرمز، لا من الطلب
public sealed record CreateDocumentCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DocumentType Type,
    string FileName,
    string FilePath,
    long FileSizeBytes,
    string ContentType) : ICommand<Guid>;