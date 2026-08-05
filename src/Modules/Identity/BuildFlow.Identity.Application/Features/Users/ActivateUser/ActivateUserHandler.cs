using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Errors;
using BuildFlow.Identity.Domain.Users;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Users.ActivateUser;

internal sealed class ActivateUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ActivateUserCommand>
{
    public async Task<Result> Handle(
        ActivateUserCommand command,
        CancellationToken cancellationToken)
    {
        // 1. جد المستخدم برمز التفعيل
        var user = await userRepository.GetByActivationTokenAsync(
            command.ActivationToken, cancellationToken);

        if (user is null)
            return Result.Fail(IdentityErrors.User.InvalidActivationToken);

        // 2. جزّئ كلمة المرور الجديدة
        var passwordHash = passwordHasher.Hash(command.NewPassword);

        // 3. فعّل الحساب: يضع كلمة المرور، ويصير نشطاً
        // الكيان يتحقّق: معلّق، ورمز صحيح، وغير منتهٍ
        var activationResult = user.ActivateWithPassword(
            command.ActivationToken, passwordHash);

        if (activationResult.IsFailed)
            return Result.Fail(activationResult.Errors);

        // 4. احفظ
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}