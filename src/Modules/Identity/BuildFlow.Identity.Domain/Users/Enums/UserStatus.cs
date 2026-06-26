namespace BuildFlow.Identity.Domain.Users.Enums;

public enum UserStatus
{
    Active = 1,    // يعمل بشكل طبيعي
    Inactive = 2,  // موقوف
    Locked = 3     // مقفل بسبب محاولات دخول فاشلة
}