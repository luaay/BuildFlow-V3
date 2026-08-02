using BuildFlow.SharedKernel.Domain.Auditing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildFlow.SharedInfrastructure.Auditing;

// الاعتراض Interceptor: يلتقط التغييرات تلقائياً عند حفظ EF Core
// يرث من معترِض حفظ التغييرات SaveChangesInterceptor
public sealed class AuditInterceptor(IServiceProvider serviceProvider)
    : SaveChangesInterceptor
{
    // إعدادات تسلسل JSON serialization options
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    // أعمدة حسّاسة نستثنيها من التدقيق excluded sensitive columns
    private static readonly HashSet<string> ExcludedColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash", "RefreshToken", "SecurityStamp"
    };

    // نعترض لحظة الحفظ SavingChanges، قبل أن يُكتَب للقاعدة
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (eventData.Context is null) return result;

        // نجلب الخدمات المطلوبة من مزوّد الخدمات service provider
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var auditRepository = serviceProvider.GetRequiredService<IAuditRepository>();

        // نبني سجلّات التدقيق من الكيانات المتغيّرة
        var entries = BuildAuditEntries(eventData.Context, httpContextAccessor);
        if (entries.Count > 0)
            await auditRepository.AddRangeAsync(entries, ct);

        return result;
    }

    // نبني سجلّ تدقيق لكل كيان تغيّر
    private List<AuditEntry> BuildAuditEntries(
        DbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        // نستخرج المستأجر tenant والفاعل user من الرمز JWT
        var tenantId = GetTenantId(httpContextAccessor);
        var userId = GetCurrentUserId(httpContextAccessor);
        // والعنوان IP والمتصفّح User-Agent من الطلب HTTP request
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        var entries = new List<AuditEntry>();

        // نمرّ على كل كيان يتتبّعه EF Core في متتبّع التغييرات ChangeTracker
        foreach (var entry in context.ChangeTracker.Entries())
        {
            // نهتمّ فقط بالمضاف والمعدّل والمحذوف
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            // لا ندقّق سجلّ التدقيق نفسه، تجنّباً للدوران
            if (entry.Entity is AuditEntry)
                continue;

            var entityName = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);
            if (entityId is null) continue;

            // نحوّل حالة EF Core إلى عمليّتنا Created/Updated/Deleted
            var action = entry.State switch
            {
                EntityState.Added => AuditAction.Created,
                EntityState.Modified => AuditAction.Updated,
                EntityState.Deleted => AuditAction.Deleted,
                _ => throw new ArgumentOutOfRangeException()
            };

            Dictionary<string, object?>? oldValues = null;
            Dictionary<string, object?>? newValues = null;
            var changedColumns = new List<string>();

            // نمرّ على خصائص الكيان properties
            foreach (var prop in entry.Properties)
            {
                var colName = prop.Metadata.Name;
                if (ExcludedColumns.Contains(colName)) continue;

                // القيم القديمة، للتعديل والحذف
                if (action == AuditAction.Deleted || action == AuditAction.Updated)
                {
                    oldValues ??= [];
                    oldValues[colName] = prop.OriginalValue;
                }

                // القيم الجديدة، للإنشاء والتعديل
                if (action == AuditAction.Created || action == AuditAction.Updated)
                {
                    newValues ??= [];
                    newValues[colName] = prop.CurrentValue;
                }

                // الأعمدة التي تغيّرت فعلاً، للتعديل فقط
                if (action == AuditAction.Updated && !Equals(prop.OriginalValue, prop.CurrentValue))
                    changedColumns.Add(colName);
            }

            // لو كان تعديلاً بلا أعمدة متغيّرة فعلاً، نتجاهله
            if (action == AuditAction.Updated && changedColumns.Count == 0)
                continue;

            entries.Add(new AuditEntry
            {
                TenantId = tenantId,
                UserId = userId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues, JsonOpts),
                NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues, JsonOpts),
                ChangedColumns = changedColumns.Count > 0 ? string.Join(",", changedColumns) : null,
                OccurredAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                // نقصّ المتصفّح إن طال، حمايةً من قيم ضخمة
                UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent
            });
        }

        return entries;
    }

    // نستخرج المستأجر tenant من مطالبة الرمز claim
    private static Guid GetTenantId(IHttpContextAccessor httpContextAccessor)
    {
        var tenantIdClaim = httpContextAccessor.HttpContext?.User
            .FindFirst("tenantId")?.Value;

        return Guid.TryParse(tenantIdClaim, out var tenantId)
            ? tenantId
            : Guid.Empty;
    }

    // نستخرج المفتاح الأساسيّ primary key للكيان نصّاً
    private static string? GetEntityId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var keyValues = entry.Metadata.FindPrimaryKey()?.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .ToArray();

        return keyValues is { Length: > 0 } ? string.Join("|", keyValues) : null;
    }

    // نستخرج الفاعل user من مطالبة الرمز، من sub أو NameIdentifier
    private static Guid? GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        var sub = httpContextAccessor.HttpContext?.User
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? httpContextAccessor.HttpContext?.User
            .FindFirst("sub")?.Value;

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}