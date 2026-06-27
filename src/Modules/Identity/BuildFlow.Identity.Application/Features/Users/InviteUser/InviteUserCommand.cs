using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Domain.Users.Enums;

namespace BuildFlow.Identity.Application.Features.Users.InviteUser;

// دعوة مستخدم جديد لمستأجر الداعي الحالي
// لاحظ: لا slug ولا tenantId — يُؤخذان من سياق الداعي (ICurrentUserService)
public sealed record InviteUserCommand(
    string Email,
    string FullName,
    string InitialPassword,
    UserRole Role) : ICommand<Guid>;