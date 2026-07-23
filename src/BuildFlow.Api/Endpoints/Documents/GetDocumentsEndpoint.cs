using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.GetDocuments;
using BuildFlow.Documents.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class GetDocumentsEndpoint
{
    public static void MapGetDocumentsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/documents", HandleAsync)
            .RequireAuthorization()
            .WithName("GetDocuments")
            .WithTags("Documents");
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? projectId = null,
        [FromQuery] DocumentStatus? status = null,
        [FromQuery] DocumentType? type = null,
        [FromQuery] string? search = null)
    {
        var query = new GetDocumentsQuery(
            page, pageSize, projectId, status, type, search);

        var result = await sender.Send(query, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
    }
}