namespace BuildFlow.Application.Abstractions;

// غلاف عام لأي قائمة مع معلومات التصفّح
// record لأنه حاوية بيانات بسيطة وغير قابلة للتغيير (immutable)
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}