using FluentValidation;

namespace BuildFlow.Projects.Application.Features.Members.AddMember;

public sealed class AddMemberValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum().WithMessage("A valid role is required.");
    }
}