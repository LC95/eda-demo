using Microsoft.Extensions.Logging;
using MST.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MST.Services.Notification.EventHandlers
{
    public class CustomerCreatedEventHandler : Domain.Abstraction.Events.EventHandler<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerCreatedEventHandler> logger;

        public CustomerCreatedEventHandler(ILogger<CustomerCreatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public override Task<bool> HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("已经成功发送通知消息。");
            return Task.FromResult(true);
        }
    }
}