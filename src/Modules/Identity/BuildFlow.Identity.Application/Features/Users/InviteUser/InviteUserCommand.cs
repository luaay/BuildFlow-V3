using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Domain.Users.Enums;

namespace BuildFlow.Identity.Application.Features.Users.InviteUser;

// دعوة مستخدم جديد لمستأجر الداعي الحالي
// لاحظ: لا slug ولا tenantId — يُؤخذان من سياق الداعي (ICurrentUserService)
// دعوة مستخدم: بلا كلمة مرور، فالمدعوّ يضعها عبر رابط التفعيل
// دعوة مستخدم: بلا كلمة مرور، فالمدعوّ يضعها عبر رابط التفعيل
public sealed record InviteUserCommand(
    string Email,
    string FullName,
    UserRole Role) : ICommand<InviteUserResult>;