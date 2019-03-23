using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain.Core
{
    public interface IEventHandler
    {
        bool CanHandle(IEvent @event);
        Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
    }

    public interface IEventHandler<in T> : IEventHandler where T : IEvent
    {
        Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);
    }
}