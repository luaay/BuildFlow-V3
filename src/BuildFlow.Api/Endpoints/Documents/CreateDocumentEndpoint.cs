using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.CreateDocument;
using BuildFlow.Documents.Domain.Enums;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class CreateDocumentEndpoint
{
    public static void MapCreateDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents", HandleAsync)
            .RequireAuthorization()
            .WithName("CreateDocument")
            .WithTags("Documents");
    }

    public sealed record CreateDocumentRequest(
        Guid ProjectId,
        string Title,
        string? Description,
        DocumentType Type,
        string FileName,
        string FilePath,
        long FileSizeBytes,
        string ContentType);

    private static async Task<IResult> HandleAsync(
        CreateDocumentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateDocumentCommand(
            request.ProjectId,
            request.Title,
            request.Description,
            request.Type,
            request.FileName,
            request.FilePath,
            request.FileSizeBytes,
            request.ContentType);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.Created($"/api/documents/{result.Value}", new { id = result.Value });
    }
}