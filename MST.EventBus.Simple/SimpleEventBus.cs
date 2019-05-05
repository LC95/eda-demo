using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.EventBus.Simple
{
    /// <summary>
    /// 简单事件总线
    /// </summary>
    public class SimpleEventBus : BaseEventBus
    {
        private readonly EventQueue _eventQueue = new EventQueue();
        private readonly ILogger<SimpleEventBus> _logger;

        public SimpleEventBus(IEventHandlerExecutionContext eventHandlerExecutionContext,
            ILogger<SimpleEventBus> logger) : base(eventHandlerExecutionContext)
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
            if (!EventHandlerExecutionContext.IsHandlerRegistered<TEvent, TEventHandler>())
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