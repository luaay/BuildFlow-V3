using BuildFlow.Application.Abstractions;

namespace BuildFlow.Identity.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Slug,
    string Email,
    string Password) : ICommand<LoginResponse>;

// نتيجة غنية: التوكن + بيانات المستخدم والمستأجر للـ Frontend
public sealed record LoginResponse(
    string AccessToken,
    Guid UserId,
    string FullName,
    string Role,
    Guid TenantId,
    string TenantSlug);