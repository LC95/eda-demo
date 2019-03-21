using MST.Domain.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MST.EventBus.Simple {
    public class PassThroughEventBus : IEventBus {
        private readonly EventQueue _eventQueue = new EventQueue();
        private readonly IEnumerable<IEventHandler> _eventHandlers;

        public PassThroughEventBus(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        private void EventQueue_EventPushed(object sender, EventProcessedEventArgs e)
            => _eventHandlers
                .Where(eh => eh.CanHandle(e.Event))
                .ToList()
                .ForEach(async eh => await eh.HandleAsync(e.Event));
        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
            => Task.Factory.StartNew(() => _eventQueue.Push(@event));



        public void Subscribe()
            => _eventQueue.EventPushed += EventQueue_EventPushed;


        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this._eventQueue.EventPushed -= EventQueue_EventPushed;
                }

                _disposedValue = true;
            }
        }
        public void Dispose() => Dispose(true);


        #endregion
    }
}
