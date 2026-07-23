using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.GetDocument;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class GetDocumentEndpoint
{
    public static void MapGetDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/documents/{documentId:guid}", HandleAsync)
            .RequireAuthorization()
            .WithName("GetDocument")
            .WithTags("Documents");
    }

    private static async Task<IResult> HandleAsync(
        Guid documentId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentQuery(documentId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Ok(result.Value);
    }
}