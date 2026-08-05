using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users.Enums;
using BuildFlow.Identity.Domain.Users.Events;
using BuildFlow.SharedKernel.Domain;
using BuildFlow.SharedKernel.Domain.Auditing;
using FluentResults;
using BuildFlow.Identity.Domain.Errors;

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

    // رمز تفعيل الدعوة activation token، ووقت انتهائه
    public string? ActivationToken { get; private set; }
    public DateTime? ActivationTokenExpiresUtc { get; private set; }

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

    // مدّة صلاحية رمز التفعيل
    private static readonly TimeSpan ActivationTokenLifetime = TimeSpan.FromDays(7);

    // إنشاء مستخدم مدعوّ، معلّق بانتظار التفعيل
    // لا كلمة مرور فعلية بعد، بل رمز تفعيل يضع به كلمته
    public static User CreateInvited(
        TenantId tenantId,
        Email email,
        string fullName,
        UserRole role,
        string activationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(activationToken);

        var user = new User
        {
            Id = UserId.New(),
            TenantId = tenantId,
            Email = email,
            // كلمة مرور نائبة، لا تُستعمل للدخول، فالحالة معلّقة
            PasswordHash = "PENDING_ACTIVATION",
            FullName = fullName.Trim(),
            Role = role,
            Status = UserStatus.Pending,
            AccessFailedCount = 0,
            ActivationToken = activationToken,
            ActivationTokenExpiresUtc = DateTime.UtcNow.Add(ActivationTokenLifetime)
        };

        user.RaiseDomainEvent(
            new UserCreatedEvent(user.Id, user.TenantId, user.Email.Value));

        return user;
    }

    // تفعيل الحساب: يضع كلمة المرور، ويصير نشطاً
    // يتحقّق أن الحساب معلّق، وأن الرمز صحيح وغير منتهٍ
    public Result ActivateWithPassword(string activationToken, string passwordHash)
    {
        if (Status != UserStatus.Pending)
            return Result.Fail(IdentityErrors.User.NotPendingActivation);

        if (ActivationToken != activationToken)
            return Result.Fail(IdentityErrors.User.InvalidActivationToken);

        if (ActivationTokenExpiresUtc < DateTime.UtcNow)
            return Result.Fail(IdentityErrors.User.ActivationTokenExpired);

        PasswordHash = passwordHash;
        Status = UserStatus.Active;
        ActivationToken = null;              // نبطل الرمز بعد الاستعمال
        ActivationTokenExpiresUtc = null;

        return Result.Ok();
    }
}