namespace BuildFlow.Identity.Application.Features.Users.InviteUser;

// نتيجة الدعوة: معرّف المستخدم، ورابط التفعيل المولّد
// في تطبيق إنتاجيّ، يُرسَل الرابط بريداً؛ هنا نُظهره للتعلّم
public sealed record InviteUserResult(
    Guid UserId,
    string ActivationToken,
    string ActivationLink);