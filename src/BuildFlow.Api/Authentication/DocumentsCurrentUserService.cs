using System.Security.Claims;
using BuildFlow.Documents.Application.Abstractions;

namespace BuildFlow.Api.Authentication;

// ينفّذ عقد المستندات، يقرأ المطالبات ويكشفها خامّة
internal sealed class DocumentsCurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid UserId => Guid.Parse(Principal?.FindFirst("sub")?.Value!);
    public Guid TenantId => Guid.Parse(Principal?.FindFirst("tenant")?.Value!);
}