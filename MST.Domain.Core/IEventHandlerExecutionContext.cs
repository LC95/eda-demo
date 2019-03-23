using System;
using System.Threading;
using System.Threading.Tasks;

namespace MST.Domain.Core
{
    public interface IEventHandlerExecutionContext
    {
        void RegisterHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>;

        void RegisterHandler(Type eventType, Type handlerType);

        bool HandlerRegistered(Type eventType, Type handlerType);

        bool HandlerRegistered<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>;

        Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}