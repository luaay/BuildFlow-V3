using BuildFlow.Application.Abstractions;

namespace BuildFlow.Documents.Application.Features.Documents.AddVersion;

public sealed record AddVersionCommand(
    Guid DocumentId,
    string FileName,
    string FilePath,
    long FileSizeBytes,
    string ContentType,
    string? ChangeNotes) : ICommand;