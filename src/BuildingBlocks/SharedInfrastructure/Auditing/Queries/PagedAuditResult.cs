namespace BuildFlow.SharedInfrastructure.Auditing.Queries;

// نتيجة مرقّمة paged لسجلّ التدقيق
public sealed record PagedAuditResult(
    IReadOnlyList<AuditEntryDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);