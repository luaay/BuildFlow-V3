using BuildFlow.Identity.Application.Features.Tenants.RegisterTenant;
using BuildFlow.Identity.Domain.Tenants.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BuildFlow.Api.Errors;

namespace BuildFlow.Api.Endpoints.Tenants;

// One endpoint class per vertical slice (REPR pattern).
// Thin wrapper: receive, dispatch, translate.
internal static class RegisterTenantEndpoint
{
    public static void MapRegisterTenantEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tenants/register", HandleAsync)
            // Registration creates the first account, so it cannot
            // itself require a token.
            .AllowAnonymous()
            .WithName("RegisterTenant")
            .WithTags("Tenants");
    }

    // Plan is an enum: the frontend sends it as an integer, and the
    // model binder maps that integer to the TenantPlan enum.
    public sealed record RegisterTenantRequest(
        string TenantName,
        string Slug,
        TenantPlan Plan,
        string OwnerEmail,
        string OwnerPassword,
        string OwnerFullName);

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterTenantRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterTenantCommand(
            request.TenantName,
            request.Slug,
            request.Plan,
            request.OwnerEmail,
            request.OwnerPassword,
            request.OwnerFullName);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
        
    }
}