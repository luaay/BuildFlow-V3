using BuildFlow.Application.Abstractions;

namespace BuildFlow.Documents.Application.Features.Documents.ApproveDocument;

public sealed record ApproveDocumentCommand(
    Guid DocumentId,
    string? Notes) : ICommand;