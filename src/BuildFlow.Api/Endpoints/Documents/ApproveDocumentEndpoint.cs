using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.ApproveDocument;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class ApproveDocumentEndpoint
{
    public static void MapApproveDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents/{documentId:guid}/approve", HandleAsync)
            .RequireAuthorization()
            .WithName("ApproveDocument")
            .WithTags("Documents");
    }

    public sealed record ApproveRequest(string? Notes);

    private static async Task<IResult> HandleAsync(
        Guid documentId,
        ApproveRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ApproveDocumentCommand(documentId, request.Notes);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}