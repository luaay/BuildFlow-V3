namespace BuildFlow.Identity.Application.Abstractions;

// عقد تجزئة كلمات المرور — التنفيذ في Infrastructure (مثلاً BCrypt)
public interface IPasswordHasher
{
    // يحوّل كلمة المرور الخام إلى hash للتخزين
    string Hash(string password);

    // يتحقّق أن كلمة المرور الخام تطابق الـ hash المخزّن
    bool Verify(string password, string passwordHash);
}