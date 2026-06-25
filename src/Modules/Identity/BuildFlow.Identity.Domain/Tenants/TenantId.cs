namespace BuildFlow.Identity.Domain.Tenants;

// strongly-typed ID للمستأجر — يلفّ Guid في نوع خاص
// readonly record struct: قيمي، غير قابل للتغيير، لا يكون null، مقارنة بالقيمة مجاناً
public readonly record struct TenantId(Guid Value)
{
    // factory لإنشاء id جديد فريد
    public static TenantId New() => new(Guid.NewGuid());

    // للعرض/التسجيل
    public override string ToString() => Value.ToString();
}