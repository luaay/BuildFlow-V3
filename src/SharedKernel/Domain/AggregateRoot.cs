namespace BuildFlow.SharedKernel.Domain;

// AggregateRoot هو "حارس البوابة" — نقطة الدخول الوحيدة للـ aggregate
// يرث كل خصائص Entity (الهوية والمساواة) ويضيف إدارة الأحداث
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    // قائمة خاصة — لا أحد خارج الـ aggregate يضيف أحداثاً مباشرة
    private readonly List<IDomainEvent> _domainEvents = [];

    // الخارج يقرأ فقط — لا يعدّل
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // protected — فقط الـ root وأبناؤه يطلقون الأحداث
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    // يُستدعى بعد حفظ التغييرات (dispatch) لتفريغ القائمة
    public void ClearDomainEvents() => _domainEvents.Clear();

    // constructor بمعامل id — يمرّره للـ Entity
    protected AggregateRoot(TId id) : base(id)
    {
    }

    // constructor فارغ — لـ EF Core
    protected AggregateRoot()
    {
    }
}