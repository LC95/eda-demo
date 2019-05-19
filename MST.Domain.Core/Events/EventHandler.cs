using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain.Abstraction.Events
{
    public abstract class EventHandler<T> : IEventHandler<T> where T : IEvent
    {
        public abstract Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);

        public bool CanHandle(IEvent @event) => typeof(T) == @event.GetType();

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return CanHandle(@event) ? HandleAsync((T) @event, cancellationToken) : Task.FromResult(false);
        }
    }
}