using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Documents.Domain.Errors;

public static class DocumentErrors
{
    public static AppError NotFound(Guid id) =>
        new("Document.NotFound", $"Document '{id}' was not found.");

    public static AppError TitleRequired =>
        new("Document.TitleRequired", "Document title is required.");

    public static AppError InvalidStatusTransition(string from, string to) =>
        new("Document.InvalidStatusTransition",
            $"Cannot change document status from '{from}' to '{to}'.");

    public static AppError CannotModifyUnderReview =>
        new("Document.CannotModifyUnderReview",
            "A document under review cannot be modified.");

    public static AppError CannotModifyFinalized =>
        new("Document.CannotModifyFinalized",
            "An approved or archived document cannot be modified.");

    public static AppError ReviewerRequired =>
        new("Document.ReviewerRequired",
            "A reviewer must be assigned before review.");

    public static AppError NotTheAssignedReviewer =>
        new("Document.NotTheAssignedReviewer",
            "Only the assigned reviewer can approve or reject.");

    public static AppError Forbidden =>
        new("Document.Forbidden",
            "You are not allowed to perform this action.");
}