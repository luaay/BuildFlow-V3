using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildFlow.Application.Abstractions.Behaviors;

// يقيس زمن كل طلب، وينبّه على البطيء
public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // العتبة: ما تجاوزها يُعدّ بطيئاً
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        // نبّه فقط على البطيء، فلا نغرق السجلّ
        if (elapsedMs > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "Slow request: {RequestName} took {ElapsedMilliseconds} ms",
                typeof(TRequest).Name,
                elapsedMs);
        }

        return response;
    }
}