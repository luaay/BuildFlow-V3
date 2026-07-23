using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.RejectDocument;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class RejectDocumentEndpoint
{
    public static void MapRejectDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents/{documentId:guid}/reject", HandleAsync)
            .RequireAuthorization()
            .WithName("RejectDocument")
            .WithTags("Documents");
    }

    public sealed record RejectRequest(string? Notes);

    private static async Task<IResult> HandleAsync(
        Guid documentId,
        RejectRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RejectDocumentCommand(documentId, request.Notes);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}