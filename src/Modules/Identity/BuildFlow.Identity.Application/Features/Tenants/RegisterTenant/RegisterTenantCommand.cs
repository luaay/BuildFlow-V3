using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Domain.Tenants.Enums;

namespace BuildFlow.Identity.Application.Features.Tenants.RegisterTenant;

// أمر تسجيل مستأجر جديد مع أول مستخدم (Owner)
public sealed record RegisterTenantCommand(
    string TenantName,
    string Slug,
    TenantPlan Plan,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFullName) : ICommand<RegisterTenantResponse>;

// النتيجة: معرّفا المستأجر والمالك
public sealed record RegisterTenantResponse(Guid TenantId, Guid OwnerUserId);