using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Errors;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Domain.Users.Enums;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Tenants.RegisterTenant;

internal sealed class RegisterTenantHandler(
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterTenantCommand, RegisterTenantResponse>
{
    public async Task<Result<RegisterTenantResponse>> Handle(
        RegisterTenantCommand command,
        CancellationToken cancellationToken)
    {
        // 1. تحقّق أن الـ slug غير مستخدم
        if (await tenantRepository.SlugExistsAsync(command.Slug, cancellationToken))
            return Result.Fail(IdentityErrors.Tenant.SlugAlreadyExists(command.Slug));

        // 2. حوّل البريد إلى value object (يتحقّق من الصيغة)
        var emailResult = Email.Create(command.OwnerEmail);
        if (emailResult.IsFailed)
            return Result.Fail(emailResult.Errors);

        // 3. أنشئ المستأجر
        var tenant = Tenant.Create(command.TenantName, command.Slug, command.Plan);

        // 4. جزّئ كلمة المرور
        var passwordHash = passwordHasher.Hash(command.OwnerPassword);

        // 5. أنشئ المستخدم المالك
        var owner = User.Create(
            tenant.Id,
            emailResult.Value,
            passwordHash,
            command.OwnerFullName,
            UserRole.Owner);

        // عند التسجيل، المالك "أنشأ نفسه" — نضبط CreatedBy لمعرّفه
        owner.CreatedBy = owner.Id.Value;

        // 6. أضف الكيانين (لا حفظ بعد)
        await tenantRepository.AddAsync(tenant, cancellationToken);
        await userRepository.AddAsync(owner, cancellationToken);

        // 7. احفظ كل شيء معاً (معاملة واحدة)
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. أرجِع المعرّفين
        return Result.Ok(new RegisterTenantResponse(tenant.Id.Value, owner.Id.Value));
    }
}