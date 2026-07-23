namespace BuildFlow.Documents.Application.Common;

// ملخّص المستند في القوائم
public sealed record DocumentSummaryDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Type,
    string Status,
    int CurrentVersionNumber,
    DateTime CreatedAtUtc);

// تفصيل المستند، مع إصداراته
public sealed record DocumentDetailDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Type,
    string Status,
    int CurrentVersionNumber,
    Guid? ReviewerId,
    DateTime? SubmittedForReviewAtUtc,
    DateTime? ReviewedAtUtc,
    string? ReviewNotes,
    IReadOnlyList<DocumentVersionDto> Versions,
    DateTime CreatedAtUtc,
    DateTime? ModifiedAtUtc);

// إصدار المستند
public sealed record DocumentVersionDto(
    Guid Id,
    int VersionNumber,
    string FileName,
    long FileSizeBytes,
    string ContentType,
    string? ChangeNotes,
    Guid UploadedBy,
    DateTime UploadedAtUtc);