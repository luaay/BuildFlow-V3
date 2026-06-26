using FluentValidation;

namespace BuildFlow.Identity.Application.Features.Tenants.RegisterTenant;

// التحقّق من مدخلات تسجيل المستأجر (الشكل والصيغة)
public sealed class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.TenantName)
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(200).WithMessage("Tenant name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug may contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("Owner email is required.")
            .EmailAddress().WithMessage("Owner email format is invalid.");

        RuleFor(x => x.OwnerPassword)
            .NotEmpty().WithMessage("Owner password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.OwnerFullName)
            .NotEmpty().WithMessage("Owner full name is required.")
            .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.");
    }
}