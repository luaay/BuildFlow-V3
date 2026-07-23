using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Documents.Infrastructure.Persistence;

internal sealed class UnitOfWork(
    DocumentsDbContext context,
    IPublisher publisher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // اجمع الأحداث قبل الحفظ
        var domainEvents = CollectDomainEvents();

        var result = await context.SaveChangesAsync(cancellationToken);

        // انشر بعد نجاح الحفظ فقط
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        return result;
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        var aggregates = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();

        return domainEvents;
    }
}