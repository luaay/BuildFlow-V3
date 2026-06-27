namespace BuildFlow.Identity.Application.Features.Users.GetUsers;

// DTO لعرض بيانات المستخدم — يكشف فقط ما يحتاجه العميل
public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Status);