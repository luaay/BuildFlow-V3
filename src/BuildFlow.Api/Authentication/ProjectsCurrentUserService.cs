using System.Security.Claims;
using BuildFlow.Projects.Application.Abstractions;

namespace BuildFlow.Api.Authentication;

// Serves the Projects module's ICurrentUserService contract, which
// expects raw Guids on the boundary. Lives in the Api host because it
// reads from HttpContext. Kept separate from the Identity implementation
// to preserve module independence.
internal sealed class ProjectsCurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    // The JwtProvider writes the user id under the "sub" claim.
    public Guid UserId => Guid.Parse(Principal?.FindFirst("sub")?.Value!);

    // The JwtProvider writes the tenant id under the "tenant" claim.
    public Guid TenantId => Guid.Parse(Principal?.FindFirst("tenant")?.Value!);
}