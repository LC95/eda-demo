using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.Domain
{
    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerCreatedEventHandler> _logger;
        private readonly IEventStore _store;

        public CustomerCreatedEventHandler(IEventStore store, ILogger<CustomerCreatedEventHandler> logger)
        {
            _store = store;
            _logger = logger;
        }

        public Task<bool> HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("CustomerCreatedEventHandler构造完毕" + GetHashCode());
            _store.SaveEventAsync(@event);
            return Task.FromResult(true);
        }

        public bool CanHandle(IEvent @event)
        {
            return @event.GetType() == typeof(CustomerCreatedEvent);
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("处理用户生成事件开始" + GetHashCode());

            var result = HandleAsync((CustomerCreatedEvent) @event, cancellationToken);

            _logger.LogInformation("处理用户生成事件完成" + GetHashCode());
            return result;
        }
    }
}