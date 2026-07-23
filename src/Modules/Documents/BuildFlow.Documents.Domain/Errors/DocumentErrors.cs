using FluentResults;

namespace BuildFlow.Documents.Domain.Errors;

public static class DocumentErrors
{
    public static Error NotFound(Guid id) =>
        new Error($"Document '{id}' was not found.")
            .WithMetadata("Code", "Document.NotFound");

    public static Error TitleRequired =>
        new Error("Document title is required.")
            .WithMetadata("Code", "Document.TitleRequired");

    public static Error InvalidStatusTransition(string from, string to) =>
        new Error($"Cannot change document status from '{from}' to '{to}'.")
            .WithMetadata("Code", "Document.InvalidStatusTransition");

    public static Error CannotModifyUnderReview =>
        new Error("A document under review cannot be modified.")
            .WithMetadata("Code", "Document.CannotModifyUnderReview");

    public static Error CannotModifyFinalized =>
        new Error("An approved or archived document cannot be modified.")
            .WithMetadata("Code", "Document.CannotModifyFinalized");

    public static Error ReviewerRequired =>
        new Error("A reviewer must be assigned before review.")
            .WithMetadata("Code", "Document.ReviewerRequired");

    public static Error NotTheAssignedReviewer =>
        new Error("Only the assigned reviewer can approve or reject.")
            .WithMetadata("Code", "Document.NotTheAssignedReviewer");

    public static Error Forbidden =>
        new Error("You are not allowed to perform this action.")
            .WithMetadata("Code", "Document.Forbidden");
}