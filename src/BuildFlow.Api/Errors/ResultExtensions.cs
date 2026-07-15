using BuildFlow.SharedKernel.Domain;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Errors;

// Central translation from a failed Result to an HTTP response.
// Endpoints call result.ToProblem() and stay free of mapping logic.
internal static class ResultExtensions
{
    public static IResult ToProblem(this ResultBase result)
    {
        // Read the first error as our structured AppError to get its Code.
        // Fall back to a generic 400 if the error is not an AppError.
        var appError = result.Errors
            .OfType<AppError>()
            .FirstOrDefault();

        if (appError is null)
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: result.Errors.FirstOrDefault()?.Message);

        var (status, title) = MapCode(appError.Code);

        // RFC 7807 ProblemDetails, with our stable error code attached
        // as an extension so the client can branch on code, not message.
        return Results.Problem(
            statusCode: status,
            title: title,
            detail: appError.Message,
            extensions: new Dictionary<string, object?>
            {
                ["code"] = appError.Code
            });
    }

    // Map by code SUFFIX so new modules inherit the behavior for free.
    // A code like "Projects.NotFound" maps to 404 without touching this.
    private static (int Status, string Title) MapCode(string code)
    {
        if (code.EndsWith("NotFound"))
            return (StatusCodes.Status404NotFound, "Resource Not Found");

        if (code.EndsWith("AlreadyExists"))
            return (StatusCodes.Status409Conflict, "Conflict");

        // Authentication-related identity errors.
        if (code is "User.InvalidCredentials"
                 or "User.AccountLocked"
                 or "User.AccountInactive")
            return (StatusCodes.Status401Unauthorized, "Unauthorized");

        // Authorization / forbidden states.
        if (code is "Tenant.Suspended" or "User.CannotInviteOwner")
            return (StatusCodes.Status403Forbidden, "Forbidden");

        // Anything unmapped is a safe generic.
        return (StatusCodes.Status400BadRequest, "Bad Request");
    }
}