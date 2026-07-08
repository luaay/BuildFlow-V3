using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.SharedKernel.Domain;
using MediatR;

namespace BuildFlow.Projects.Infrastructure.Persistence;

internal sealed class UnitOfWork(
    ProjectsDbContext context,
    IPublisher publisher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1. اجمع الأحداث من كل التجميعات المتتبَّعة قبل الحفظ
        var domainEvents = CollectDomainEvents();

        // 2. احفظ التغييرات في قاعدة البيانات
        var result = await context.SaveChangesAsync(ct);

        // 3. بعد نجاح الحفظ، انشر الأحداث
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        return result;
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        // جد كل جذور التجميع المتتبَّعة التي لها أحداث
        var aggregates = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        // اجمع أحداثها
        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // نظّف الأحداث من التجميعات لئلا تُنشر مجدّداً
        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();

        return domainEvents;
    }
}