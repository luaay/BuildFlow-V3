using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Identity.Domain.Errors;

// تعريف مركزي لأخطاء module الـ Identity — كل خطأ بكود ورسالة
public static class IdentityErrors
{
    public static class Tenant
    {
        public static AppError SlugAlreadyExists(string slug) =>
            new("Tenant.SlugAlreadyExists",
                $"A tenant with slug '{slug}' already exists.");

        public static AppError NotFound =>
            new("Tenant.NotFound", "Tenant was not found.");
    }

    public static class User
    {
        public static AppError EmailAlreadyExists =>
            new("User.EmailAlreadyExists",
                "A user with this email already exists.");

        public static AppError NotFound =>
            new("User.NotFound", "User was not found.");

        public static AppError InvalidCredentials =>
            new("User.InvalidCredentials", "Invalid email or password.");

        public static AppError AccountLocked =>
            new("User.AccountLocked",
                "Account is locked due to multiple failed login attempts.");

        public static AppError AccountInactive =>
            new("User.AccountInactive", "Account is inactive.");
    }
}