using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Identity.Infrastructure.Persistence;

internal sealed class UnitOfWork(
    IdentityDbContext context,
    IPublisher publisher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. اجمع الأحداث من كل الـ aggregates المتتبَّعة قبل الحفظ
        var domainEvents = CollectDomainEvents();

        // 2. احفظ التغييرات في قاعدة البيانات
        var result = await context.SaveChangesAsync(cancellationToken);

        // 3. بعد نجاح الحفظ — انشر الأحداث
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        return result;
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        // جد كل الـ aggregate roots المتتبَّعة التي لها أحداث
        var aggregates = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        // اجمع أحداثها
        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // نظّف الأحداث من الـ aggregates (لئلا تُنشر مجدداً)
        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();

        return domainEvents;
    }
}