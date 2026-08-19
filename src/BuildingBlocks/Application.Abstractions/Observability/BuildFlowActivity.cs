using System.Diagnostics;

namespace BuildFlow.Application.Abstractions.Observability;

public static class BuildFlowActivity
{
    public const string ServiceName = "BuildFlow.Api";

    public static readonly ActivitySource Source = new(ServiceName);
}