using System.Security.Claims;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;

namespace BuildFlow.Api.Authentication;

// Reads the authenticated identity from the current HTTP request.
// Lives in the Api host because it depends on HttpContext, which only
// exists here. The Application layer defines the contract; the host
// provides the implementation.
internal sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    // True only when a valid authenticated identity is attached.
    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ?? false;

    // The JwtProvider writes the user id under the "sub" claim.
    // We read the same key, then wrap the raw Guid back into the
    // strongly-typed UserId that the domain expects.
    public UserId UserId
    {
        get
        {
            var raw = Principal?.FindFirst("sub")?.Value;
            return new UserId(Guid.Parse(raw!));
        }
    }

    // The JwtProvider writes the tenant id under the "tenant" claim.
    public TenantId TenantId
    {
        get
        {
            var raw = Principal?.FindFirst("tenant")?.Value;
            return new TenantId(Guid.Parse(raw!));
        }
    }
}