using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Ordering.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Ordering.Infrastructure.Data.Interceptors
{
   //    The primary purpose of this interceptor is to:

   //   Intercept the SaveChanges process (SavingChanges and SavingChangesAsync methods).
   //   Find all aggregate root entities that have unprocessed domain events.
   //   Collect all domain events from these entities.
   //   Dispatch each domain event using MediatR (mediator.Publish(domainEvent)).
   //   Clear the domain events from the aggregates after dispatching.

    public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);   //we are using base.SavingChanges which means we will be executing in the base pipeline only. so no recursive calls.
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public async Task DispatchDomainEvents(DbContext? context)  //ensures that domain events are dispatched when changes to aggregate entities are saved to the database.
        {
            if (context == null) return;

            var aggregates = context.ChangeTracker       
                .Entries<IAggregate>()                           // Find all entities that implement IAggregate interface
                .Where(a => a.Entity.DomainEvents.Any())         // Only select those with domain events
                .Select(a => a.Entity);

            var domainEvents = aggregates              
                .SelectMany(a => a.DomainEvents)                // Collect all domain events, like the Order Aggregate has OrderCreatedEvent, OrderUpdateEvent
                .ToList();

            aggregates.ToList().ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);            // Dispatch events using MediatR
            }
        }
    }
}
