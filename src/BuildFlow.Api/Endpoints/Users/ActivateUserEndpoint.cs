using BuildFlow.Api.Errors;
using BuildFlow.Identity.Application.Features.Users.ActivateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Users;

// نقطة تفعيل الحساب، مفتوحة، فالمدعوّ لا يملك جلسة بعد
// يضع كلمة مروره بالرمز، فيصير حسابه نشطاً
internal static class ActivateUserEndpoint
{
    public static void MapActivateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/activate", HandleAsync)
            .WithName("ActivateUser")
            .WithTags("Users");
    }

    public sealed record ActivateUserRequest(
        string ActivationToken,
        string NewPassword);

    private static async Task<IResult> HandleAsync(
        [FromBody] ActivateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(
            request.ActivationToken,
            request.NewPassword);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(new { message = "Account activated. You can now sign in." });
    }
}