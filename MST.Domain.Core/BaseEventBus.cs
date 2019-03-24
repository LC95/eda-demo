using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain.Core
{
    public abstract class BaseEventBus : IEventBus
    {
        protected readonly IEventHandlerExecutionContext EventHandlerExecutionContext;

        protected BaseEventBus(IEventHandlerExecutionContext eventHandlerExecutionContext)
        {
            EventHandlerExecutionContext = eventHandlerExecutionContext;
        }


        public abstract void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>;


        public abstract Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;


        #region Dispose

        private bool _disposedValue;

        protected abstract void DisposeObject();

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) DisposeObject();

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}