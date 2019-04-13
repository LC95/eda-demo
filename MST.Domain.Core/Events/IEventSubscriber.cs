using System;

namespace MST.Domain.Abstraction
{
    public interface IEventSubscriber : IDisposable
    {
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;
    }
}