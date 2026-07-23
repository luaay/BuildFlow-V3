using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.AddVersion;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class AddVersionEndpoint
{
    public static void MapAddVersionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents/{documentId:guid}/versions", HandleAsync)
            .RequireAuthorization()
            .WithName("AddVersion")
            .WithTags("Documents");
    }

    public sealed record AddVersionRequest(
        string FileName,
        string FilePath,
        long FileSizeBytes,
        string ContentType,
        string? ChangeNotes);

    private static async Task<IResult> HandleAsync(
        Guid documentId,
        AddVersionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddVersionCommand(
            documentId,
            request.FileName,
            request.FilePath,
            request.FileSizeBytes,
            request.ContentType,
            request.ChangeNotes);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}