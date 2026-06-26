namespace BuildFlow.Identity.Domain.Users;

// strongly-typed ID للمستخدم
public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}