using BuildFlow.Api.Errors;
using BuildFlow.Identity.Application.Features.Users.InviteUser;
using BuildFlow.Identity.Domain.Users.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Users;

// نقطة دعوة مستخدم، محميّة. المستأجر والداعي من الرمز، لا من الجسم.
// المدعوّ يُنشأ معلّقاً، ويصله رابط تفعيل يضع به كلمة مروره.
internal static class InviteUserEndpoint
{
    public static void MapInviteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/invite", HandleAsync)
            .RequireAuthorization()
            .WithName("InviteUser")
            .WithTags("Users");
    }

    // جسم الطلب: بلا كلمة مرور، فالمدعوّ يضعها لاحقاً
    public sealed record InviteUserRequest(
        string Email,
        string FullName,
        UserRole Role);

    private static async Task<IResult> HandleAsync(
        [FromBody] InviteUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new InviteUserCommand(
            request.Email,
            request.FullName,
            request.Role);

        var result = await sender.Send(command, cancellationToken);

        // عند النجاح، نرجع رابط التفعيل ليأخذه المطوّر ويجرّبه
        // في الإنتاج، يُرسَل الرابط بريداً بدل عرضه
        return result.IsFailed
            ? result.ToProblem()
            : Results.Created(
                $"/api/users/{result.Value.UserId}",
                new
                {
                    id = result.Value.UserId,
                    activationLink = result.Value.ActivationLink
                });
    }
}