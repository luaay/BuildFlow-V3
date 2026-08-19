using System.Diagnostics;
using BuildFlow.Application.Abstractions.Observability;
using FluentResults;
using MediatR;

namespace BuildFlow.Application.Abstractions.Behaviors;

// ينشئ نطاق تتبّع لكل أمر واستعلام يمرّ في الخط
public sealed class TracingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        using var activity = BuildFlowActivity.Source.StartActivity(
            $"Mediator {requestName}",
            ActivityKind.Internal);

        activity?.SetTag("mediator.request_type", requestName);
        activity?.SetTag(
            "mediator.request_kind",
            requestName.EndsWith("Command") ? "command" : "query");

        try
        {
            var response = await next();

            activity?.SetTag(
                "mediator.result",
                response.IsSuccess ? "success" : "failure");

            if (response.IsFailed)
            {
                activity?.SetTag(
                    "mediator.error",
                    response.Errors.FirstOrDefault()?.Message);
            }

            return response;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.SetTag("exception.type", exception.GetType().FullName);
            throw;
        }
    }
}