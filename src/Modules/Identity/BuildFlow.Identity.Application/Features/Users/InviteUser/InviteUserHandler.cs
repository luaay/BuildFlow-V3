using BuildFlow.Application.Abstractions;
using BuildFlow.Application.Abstractions.Caching;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Errors;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Domain.Users.Enums;
using FluentResults;
using BuildFlow.Application.Abstractions.Configuration;
using Microsoft.Extensions.Options;

namespace BuildFlow.Identity.Application.Features.Users.InviteUser;

internal sealed class InviteUserHandler(
    ICurrentUserService currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICacheService cache,
    IOptions<FrontendOptions> frontendOptions)
    : ICommandHandler<InviteUserCommand, InviteUserResult>
{
    
    public async Task<Result<InviteUserResult>> Handle(
        InviteUserCommand command,
        CancellationToken cancellationToken)
    {
        // قاعدة: لا يُدعى المالك
        if (command.Role == UserRole.Owner)
            return Result.Fail(IdentityErrors.User.CannotInviteOwner);

        var tenantId = currentUser.TenantId;

        // 1. حوّل البريد
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailed)
            return Result.Fail(emailResult.Errors);

        var email = emailResult.Value;

        // 2. تحقّق أن البريد غير مستخدم
        if (await userRepository.EmailExistsAsync(tenantId, email, cancellationToken))
            return Result.Fail(IdentityErrors.User.EmailAlreadyExists);

        // 3. ولّد رمز تفعيل آمن تشفيرياً
        var activationToken = Convert.ToHexString(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

        // 4. أنشئ مستخدماً معلّقاً، بلا كلمة مرور، برمز التفعيل
        var user = User.CreateInvited(
            tenantId,
            email,
            command.FullName,
            command.Role,
            activationToken);

        user.CreatedBy = currentUser.UserId.Value;

        // 5. أضف واحفظ
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. العضو الجديد يُبطل كل صفحات قائمة المستخدمين لهذا المستأجر
        await cache.RemoveByPrefixAsync(
            UserCacheKeys.TenantPrefix(tenantId.Value), cancellationToken);

       
        // 7. ابنِ رابط التفعيل من الإعدادات
        var activationLink =
            $"{frontendOptions.Value.BaseUrl}/activate?token={activationToken}";

        return Result.Ok(new InviteUserResult(
            user.Id.Value, activationToken, activationLink));
    }
}