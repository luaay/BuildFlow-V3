namespace BuildFlow.Projects.Domain.Entities;

public readonly record struct ProjectMemberId(Guid Value)
{
    public static ProjectMemberId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}