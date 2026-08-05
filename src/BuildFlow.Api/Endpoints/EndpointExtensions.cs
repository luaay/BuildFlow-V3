using BuildFlow.Api.Endpoints.Auth;
using BuildFlow.Api.Endpoints.Projects;
using BuildFlow.Api.Endpoints.Tenants;
using BuildFlow.Api.Endpoints.Users;
using BuildFlow.Api.Endpoints.Documents;
using BuildFlow.Api.Endpoints.Audit;

namespace BuildFlow.Api.Endpoints;

// Aggregates all slice endpoint registrations behind one call,
// so Program.cs stays clean as endpoints grow.
internal static class EndpointExtensions
{
    public static void MapBuildFlowEndpoints(this IEndpointRouteBuilder app)
    {
        // Identity module
        app.MapRegisterTenantEndpoint();
        app.MapLoginEndpoint();
        app.MapGetUsersEndpoint();
        app.MapInviteUserEndpoint();

        // Projects module
        // Projects module
        app.MapCreateProjectEndpoint();
        app.MapGetProjectsEndpoint();
        app.MapGetProjectEndpoint();
        app.MapUpdateProjectEndpoint();
        app.MapChangeProjectStatusEndpoint();
        app.MapAddMemberEndpoint();
        app.MapRemoveMemberEndpoint();


        // Documents module
        app.MapCreateDocumentEndpoint();
        app.MapGetDocumentsEndpoint();
        app.MapGetDocumentEndpoint();
        app.MapSubmitForReviewEndpoint();
        app.MapApproveDocumentEndpoint();
        app.MapRejectDocumentEndpoint();
        app.MapAddVersionEndpoint();

        // Audit module
        app.MapGetAuditLogEndpoint();


        app.MapActivateUserEndpoint();
    }
}