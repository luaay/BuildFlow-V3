using BuildFlow.SharedKernel.Domain.Auditing;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BuildFlow.SharedInfrastructure.Auditing.Queries;

// معالج استعلام سجلّ التدقيق Query handler
// يقرأ المستأجر من الطلب، ويستدعي المستودع، ويبني النتيجة المرقّمة
public sealed class GetAuditLogQueryHandler(
    IAuditRepository auditRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAuditLogQuery, PagedAuditResult>
{
    public async Task<PagedAuditResult> Handle(
        GetAuditLogQuery request,
        CancellationToken cancellationToken)
    {
        // نقرأ المستأجر الحاليّ من مطالبة الرمز claim، كما يفعل الاعتراض
        var tenantId = GetTenantId();

        // نجلب الصفحة، والعدد الكلّيّ
        var entries = await auditRepository.GetByTenantAsync(
            tenantId, request.Page, request.PageSize, cancellationToken);

        var totalCount = await auditRepository.CountByTenantAsync(
            tenantId, cancellationToken);

        // نحسب عدد الصفحات
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // نحوّل الكيانات إلى كائنات نقل
        var items = entries.Select(e => e.ToDto()).ToList();

        return new PagedAuditResult(
            Items: items,
            TotalCount: totalCount,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalPages: totalPages,
            HasNextPage: request.Page < totalPages,
            HasPreviousPage: request.Page > 1);
    }

    // نستخرج المستأجر tenant من مطالبة الرمز، كما في الاعتراض
    private Guid GetTenantId()
    {
        var tenantIdClaim = httpContextAccessor.HttpContext?.User
            .FindFirst("tenant")?.Value;

        return Guid.TryParse(tenantIdClaim, out var tenantId)
            ? tenantId
            : Guid.Empty;
    }
}