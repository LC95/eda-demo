using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;

namespace MST.Domain
{
    public class CustomerCreateEventAnotherHandler : IEventHandler<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerCreateEventAnotherHandler> _logger;

        public CustomerCreateEventAnotherHandler(ILogger<CustomerCreateEventAnotherHandler> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var result = Task.FromResult(true);
            return result;
        }

        public bool CanHandle(IEvent @event)
        {
            return @event.GetType() == typeof(CustomerCreatedEvent);
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("另一个处理器开始处理" + GetHashCode());

            var result = HandleAsync((CustomerCreatedEvent) @event, cancellationToken);

            _logger.LogInformation("另一个处理器处理完成" + GetHashCode());
            return result;
        }
    }
}