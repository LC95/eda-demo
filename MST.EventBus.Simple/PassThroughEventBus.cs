using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Core;

namespace MST.EventBus.Simple
{
    public class PassThroughEventBus : IEventBus
    {
        private readonly IEventHandlerExecutionContext _context;
        private readonly EventQueue _eventQueue = new EventQueue();
        private readonly ILogger<PassThroughEventBus> _logger;

        public PassThroughEventBus(ILogger<PassThroughEventBus> logger,
            IEventHandlerExecutionContext context)
        {
            _logger = logger;
            _context = context;
            _eventQueue.EventPushed += EventQueue_EventPushed;
            _logger.LogInformation("PassThroughEventBus 构造完毕");
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            return Task.Factory.StartNew(() => _eventQueue.Push(@event));
        }

        private async void EventQueue_EventPushed(object sender, EventProcessedEventArgs e)
        {
            await _context.HandleEventAsync(e.Event);
        }


        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) _eventQueue.EventPushed -= EventQueue_EventPushed;

                _logger.LogInformation("PassThroughEventBus 被回收");
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Subscribe<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>
        {
            if (!_context.HandlerRegistered<TEvent, TEventHandler>()) 
                _context.RegisterHandler<TEvent, TEventHandler>();
        }

        #endregion
    }
}