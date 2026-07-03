using BuildFlow.Api.Endpoints.Auth;
using BuildFlow.Api.Endpoints.Tenants;
using BuildFlow.Api.Endpoints.Users;

namespace BuildFlow.Api.Endpoints;

// Aggregates all slice endpoint registrations behind one call,
// so Program.cs stays clean as endpoints grow.
internal static class EndpointExtensions
{
    public static void MapBuildFlowEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterTenantEndpoint();
        app.MapLoginEndpoint();
        app.MapGetUsersEndpoint();
        app.MapInviteUserEndpoint();
        // Future slices register their endpoints here.

       
    }
}