using System;

namespace MST.Domain.Abstraction.Events
{
    public interface IEventSubscriber : IDisposable
    {
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;
    }
}