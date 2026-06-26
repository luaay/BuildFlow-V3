namespace BuildFlow.Identity.Application.Abstractions;

// عقد حفظ التغييرات في معاملة واحدة — التنفيذ يلفّ DbContext في Infrastructure
public interface IUnitOfWork
{
    // يحفظ كل التغييرات المعلّقة، ويُرجع عدد السجلّات المتأثّرة
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}