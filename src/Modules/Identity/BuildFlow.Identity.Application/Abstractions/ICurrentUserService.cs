using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;

namespace BuildFlow.Identity.Application.Abstractions;

// عقد للوصول لهوية المستخدم الحالي — التنفيذ في طبقة Api
// يكشف فقط ما تحتاجه طبقة Application، لا كامل الـ HTTP context
public interface ICurrentUserService
{
    UserId UserId { get; }
    TenantId TenantId { get; }
    bool IsAuthenticated { get; }
}