using BuildFlow.Api.Errors;
using BuildFlow.Documents.Application.Features.Documents.SubmitForReview;
using MediatR;

namespace BuildFlow.Api.Endpoints.Documents;

internal static class SubmitForReviewEndpoint
{
    public static void MapSubmitForReviewEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents/{documentId:guid}/submit-for-review", HandleAsync)
            .RequireAuthorization()
            .WithName("SubmitForReview")
            .WithTags("Documents");
    }

    public sealed record SubmitForReviewRequest(Guid ReviewerId);

    private static async Task<IResult> HandleAsync(
        Guid documentId,
        SubmitForReviewRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SubmitForReviewCommand(documentId, request.ReviewerId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailed
            ? result.ToProblem()
            : Results.NoContent();
    }
}