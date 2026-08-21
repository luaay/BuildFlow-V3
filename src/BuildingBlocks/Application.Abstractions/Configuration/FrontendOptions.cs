namespace BuildFlow.Application.Abstractions.Configuration;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    public string BaseUrl { get; init; } = string.Empty;
}