using Serilog.Context;

namespace BuildFlow.Api.Logging;

// يضيف هوية الطالب إلى سياق السجلّ، فترافق كل سطر ضمن الطلب
internal sealed class RequestContextEnrichmentMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        // أضِف الخصائص فقط إن كان الطلب مصادَقاً
        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirst("sub")?.Value;
            var tenantId = user.FindFirst("tenant")?.Value;

            // ادفع الخصائص إلى سياق السجلّ لمدّة هذا الطلب
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("TenantId", tenantId))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}