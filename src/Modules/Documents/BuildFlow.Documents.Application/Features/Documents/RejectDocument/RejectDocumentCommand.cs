using BuildFlow.Application.Abstractions;

namespace BuildFlow.Documents.Application.Features.Documents.RejectDocument;

public sealed record RejectDocumentCommand(
    Guid DocumentId,
    string? Notes) : ICommand;