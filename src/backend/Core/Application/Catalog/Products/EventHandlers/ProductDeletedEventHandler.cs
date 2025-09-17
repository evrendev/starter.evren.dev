using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Products.EventHandlers;

public class ProductDeletedEventHandler
    (ILogger<ProductDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<Product>>
{
    public override Task Handle(EntityDeletedEvent<Product> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
