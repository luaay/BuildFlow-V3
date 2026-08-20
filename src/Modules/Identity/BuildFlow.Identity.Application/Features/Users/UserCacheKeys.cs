namespace BuildFlow.Identity.Application.Features.Users;

internal static class UserCacheKeys
{
    // بادئة كل مفاتيح مستخدمي المستأجر — عليها يقع الإبطال
    public static string TenantPrefix(Guid tenantId)
        => $"tenant:{tenantId}:users:";

    public static string List(Guid tenantId, int page, int pageSize)
        => $"{TenantPrefix(tenantId)}list:p{page}:s{pageSize}";
}