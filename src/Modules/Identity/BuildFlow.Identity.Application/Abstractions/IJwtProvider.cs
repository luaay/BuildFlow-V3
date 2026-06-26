using BuildFlow.Identity.Domain.Users;

namespace BuildFlow.Identity.Application.Abstractions;

// عقد إصدار JWT — التنفيذ في Infrastructure
public interface IJwtProvider
{
    // يصدر توكن JWT للمستخدم (يضمّن UserId, TenantId, Role في الـ claims)
    string GenerateToken(User user);
}