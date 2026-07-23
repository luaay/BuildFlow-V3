using BuildFlow.Application.Abstractions;

namespace BuildFlow.Documents.Application.Features.Documents.SubmitForReview;

public sealed record SubmitForReviewCommand(
    Guid DocumentId,
    Guid ReviewerId) : ICommand;