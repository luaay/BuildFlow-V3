using BuildFlow.SharedInfrastructure.Auditing.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Audit;

// نقطة جلب سجلّ التدقيق، على نمط REPR كبقيّة النقاط
// محميّة: تحتاج رمزاq. المستأجر يُقرأ من الرمز في المعالج، لا يُمرَّر
internal static class GetAuditLogEndpoint
{
    public static void MapGetAuditLogEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/audit", HandleAsync)
            .RequireAuthorization()
            .WithName("GetAuditLog")
            .WithTags("Audit");
    }

    // الترقيم من سلسلة الاستعلام، بقيم افتراضية
    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAuditLogQuery(page, pageSize);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}