using BuildFlow.Application.Abstractions;

namespace BuildFlow.Identity.Application.Features.Users.ActivateUser;

// تفعيل حساب مدعوّ: يضع المدعوّ كلمة مروره بالرمز
// لا مصادقة، فالمدعوّ لا يملك جلسة بعد
public sealed record ActivateUserCommand(
    string ActivationToken,
    string NewPassword) : ICommand;