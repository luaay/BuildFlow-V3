using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Errors;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Tenants.Enums;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Domain.Users.Enums;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Auth.Login;

internal sealed class LoginHandler(
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // 1. جد المستأجر بالـ slug
        var tenant = await tenantRepository.GetBySlugAsync(command.Slug, cancellationToken);
        if (tenant is null)
            return Result.Fail(IdentityErrors.User.InvalidCredentials);

        // فحص حالة المستأجر — رسالة صريحة (قرارك)
        if (tenant.Status == TenantStatus.Suspended)
            return Result.Fail(IdentityErrors.Tenant.Suspended);

        // 2. حوّل البريد إلى value object
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailed)
            return Result.Fail(IdentityErrors.User.InvalidCredentials);

        // 3. جد المستخدم بالبريد ضمن المستأجر
        var user = await userRepository.GetByEmailAsync(
            tenant.Id, emailResult.Value, cancellationToken);
        if (user is null)
            return Result.Fail(IdentityErrors.User.InvalidCredentials);

        // 4. فحص القفل أولاً
        if (user.IsLockedOut())
            return Result.Fail(IdentityErrors.User.AccountLocked);

        // 5. فحص الحالة
        if (user.Status == UserStatus.Inactive)
            return Result.Fail(IdentityErrors.User.AccountInactive);

        // 6. تحقّق من كلمة المرور
        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();           // قد يقفل الحساب
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Fail(IdentityErrors.User.InvalidCredentials);
        }

        // 7. نجاح — صفّر العدّاد وأصدر التوكن
        user.RecordSuccessfulLogin();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtProvider.GenerateToken(user);

        return Result.Ok(new LoginResponse(
            token,
            user.Id.Value,
            user.FullName,
            user.Role.ToString(),
            tenant.Id.Value,
            tenant.Slug));
    }
}