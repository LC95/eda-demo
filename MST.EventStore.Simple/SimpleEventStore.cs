using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.EventStore.Simple
{
    public class SimpleEventStore : IEventStore
    {
        private readonly ILogger<SimpleEventStore> _logger;

        public SimpleEventStore(ILogger<SimpleEventStore> logger)
        {
            logger.LogInformation("事件存储开始创建");
            _logger = logger;
            logger.LogInformation("事件存储创建完毕");
        }


        public void Dispose()
        {
            _logger.LogInformation("EventStore Disposed");
        }

        public Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            _logger.LogInformation(@event.Id + " " + @event.TimeStamp);
            return Task.CompletedTask;
        }
    }
}