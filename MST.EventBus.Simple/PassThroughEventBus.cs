using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;

namespace MST.EventBus.Simple
{
    public class PassThroughEventBus : BaseEventBus
    {
        private readonly EventQueue _eventQueue = new EventQueue();
        private readonly ILogger<PassThroughEventBus> _logger;

        public PassThroughEventBus(IEventHandlerExecutionContext eventHandlerExecutionContext,
            ILogger<PassThroughEventBus> logger) : base(eventHandlerExecutionContext)
        {
            _logger = logger;
            _eventQueue.EventPushed += EventQueue_EventPushed;
        }

        public override Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => _eventQueue.Push(@event), cancellationToken);
        }

        private async void EventQueue_EventPushed(object sender, EventProcessedEventArgs e)
        {
            await EventHandlerExecutionContext.HandleEventAsync(e.Event);
        }

        public override void Subscribe<TEvent, TEventHandler>()
        {
            if (!EventHandlerExecutionContext.HandlerRegistered<TEvent, TEventHandler>())
                EventHandlerExecutionContext.RegisterHandler<TEvent, TEventHandler>();
        }

        #region IDisposable Support

        protected override void DisposeObject()
        {
            _logger.LogInformation($"BaseEventBus {GetHashCode()} 被释放");
        }

        #endregion
    }
}