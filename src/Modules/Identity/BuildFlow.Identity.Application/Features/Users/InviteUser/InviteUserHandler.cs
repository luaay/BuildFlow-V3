using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Errors;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Domain.Users.Enums;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Users.InviteUser;

internal sealed class InviteUserHandler(
    ICurrentUserService currentUser,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<InviteUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        InviteUserCommand command,
        CancellationToken cancellationToken)
    {
        // قاعدة عمل: لا يُمنح دور Owner عبر الدعوة (المالك يُنشأ عند التسجيل فقط)
        if (command.Role == UserRole.Owner)
            return Result.Fail(IdentityErrors.User.CannotInviteOwner);

        // المستأجر من سياق الداعي (ICurrentUserService) — لا من الطلب
        var tenantId = currentUser.TenantId;

        // 1. حوّل البريد إلى value object
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailed)
            return Result.Fail(emailResult.Errors);

        var email = emailResult.Value;

        // 2. تحقّق أن البريد غير مستخدم في هذا المستأجر
        if (await userRepository.EmailExistsAsync(tenantId, email, cancellationToken))
            return Result.Fail(IdentityErrors.User.EmailAlreadyExists);

        // 3. جزّئ كلمة المرور الأوّلية
        var passwordHash = passwordHasher.Hash(command.InitialPassword);

        // 4. أنشئ المستخدم المدعوّ
        var user = User.Create(
            tenantId,
            email,
            passwordHash,
            command.FullName,
            command.Role);

        // الداعي هو من أنشأ هذا المستخدم (audit)
        user.CreatedBy = currentUser.UserId.Value;

        // 5. أضف واحفظ
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(user.Id.Value);
    }
}