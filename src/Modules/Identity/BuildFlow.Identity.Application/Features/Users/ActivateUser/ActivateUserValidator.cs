using FluentValidation;

namespace BuildFlow.Identity.Application.Features.Users.ActivateUser;

public sealed class ActivateUserValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.ActivationToken)
            .NotEmpty().WithMessage("Activation token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}