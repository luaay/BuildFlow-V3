namespace BuildFlow.Projects.Domain.Entities;

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}