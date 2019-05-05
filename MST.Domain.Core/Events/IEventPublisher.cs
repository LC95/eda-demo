using System;
using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain.Abstraction.Events
{
    public interface IEventPublisher : IDisposable
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }
}