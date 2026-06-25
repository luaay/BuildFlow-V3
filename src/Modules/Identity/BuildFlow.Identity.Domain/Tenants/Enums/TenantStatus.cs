namespace BuildFlow.Identity.Domain.Tenants.Enums;

public enum TenantStatus
{
    Active = 1,      // تعمل بشكل طبيعي
    Suspended = 2,   // موقوفة مؤقتاً (عدم الدفع مثلاً)
}