using BuildFlow.Identity.Domain.Tenants.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildFlow.Identity.Application.EventHandlers;

// يتفاعل مع إنشاء مستأجر جديد
internal sealed class TenantCreatedEventHandler(
    ILogger<TenantCreatedEventHandler> logger)
    : INotificationHandler<TenantCreatedEvent>
{
    public Task Handle(TenantCreatedEvent notification, CancellationToken cancellationToken)
    {
        // الآن: تسجيل فقط. لاحقاً: بريد ترحيب، إعدادات افتراضية، إلخ
        logger.LogInformation(
            "Tenant created: {TenantId} ({Name}) at {OccurredOn}",
            notification.TenantId,
            notification.Name,
            notification.OccurredOnUtc);

        return Task.CompletedTask;
    }
}