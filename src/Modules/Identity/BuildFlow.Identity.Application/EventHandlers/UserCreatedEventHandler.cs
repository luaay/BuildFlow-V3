using BuildFlow.Identity.Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildFlow.Identity.Application.EventHandlers;

// يتفاعل مع إنشاء مستخدم جديد
internal sealed class UserCreatedEventHandler(
    ILogger<UserCreatedEventHandler> logger)
    : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        // الآن: تسجيل. لاحقاً: بريد ترحيب/تفعيل للمستخدم
        logger.LogInformation(
            "User created: {UserId} in tenant {TenantId} ({Email})",
            notification.UserId,
            notification.TenantId,
            notification.Email);

        return Task.CompletedTask;
    }
}