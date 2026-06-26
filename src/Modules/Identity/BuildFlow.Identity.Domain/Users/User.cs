using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users.Enums;
using BuildFlow.Identity.Domain.Users.Events;
using BuildFlow.SharedKernel.Domain;
using BuildFlow.SharedKernel.Domain.Auditing;

namespace BuildFlow.Identity.Domain.Users;

public sealed class User : AggregateRoot<UserId>, IAuditableEntity, ISoftDelete
{
    public TenantId TenantId { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }

    // تتبّع محاولات الدخول الفاشلة (للقفل المؤقّت)
    public int AccessFailedCount { get; private set; }
    public DateTime? LockoutEndUtc { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    // private constructor — لـ EF Core
    private User() { }

    // Factory Method — الطريقة الوحيدة لإنشاء User
    public static User Create(
        TenantId tenantId,
        Email email,
        string passwordHash,
        string fullName,
        UserRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        var user = new User
        {
            Id = UserId.New(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            Role = role,
            Status = UserStatus.Active,
            AccessFailedCount = 0
        };

        user.RaiseDomainEvent(
            new UserCreatedEvent(user.Id, user.TenantId, user.Email.Value));

        return user;
    }

    // ثوابت قواعد القفل
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    // تسجيل محاولة دخول فاشلة — قد تؤدي للقفل
    public void RecordFailedLogin()
    {
        AccessFailedCount++;

        if (AccessFailedCount >= MaxFailedAttempts)
        {
            Status = UserStatus.Locked;
            LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
        }
    }

    // تسجيل دخول ناجح — يصفّر العدّاد
    public void RecordSuccessfulLogin()
    {
        AccessFailedCount = 0;
        LockoutEndUtc = null;

        if (Status == UserStatus.Locked)
            Status = UserStatus.Active;
    }

    // هل الحساب مقفل حالياً؟ (يراعي انتهاء مدّة القفل)
    public bool IsLockedOut()
    {
        if (Status != UserStatus.Locked)
            return false;

        // إن انتهت مدّة القفل، لم يعد مقفلاً فعلياً
        return LockoutEndUtc.HasValue && LockoutEndUtc.Value > DateTime.UtcNow;
    }

    // تغيير دور المستخدم
    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    // تعطيل المستخدم
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    // إعادة تفعيل المستخدم
    public void Activate()
    {
        Status = UserStatus.Active;
        AccessFailedCount = 0;
        LockoutEndUtc = null;
    }
}