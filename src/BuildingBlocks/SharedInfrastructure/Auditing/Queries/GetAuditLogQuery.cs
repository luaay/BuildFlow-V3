using MediatR;

namespace BuildFlow.SharedInfrastructure.Auditing.Queries;

// استعلام جلب سجلّ التدقيق، مرقّماq
// يطبّق واجهة الطلب IRequest من MediatR، ويرجع النتيجة المرقّمة
public sealed record GetAuditLogQuery(int Page, int PageSize)
    : IRequest<PagedAuditResult>;