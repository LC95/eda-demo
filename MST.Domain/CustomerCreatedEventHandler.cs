using MST.Domain.Core;
using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain {
    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
    {
        private readonly IEventStore _store;

        public CustomerCreatedEventHandler()
        {
                
        }
        public Task<bool> HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public bool CanHandle(IEvent @event)
        {
            return @event.GetType() == typeof(CustomerCreatedEvent);
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return CanHandle(@event) ? HandleAsync((CustomerCreatedEvent)@event, cancellationToken) : Task.FromResult(false);
        }
    }
}
