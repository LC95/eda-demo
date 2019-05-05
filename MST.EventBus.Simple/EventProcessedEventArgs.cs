using System;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.EventBus.Simple
{
    public class EventProcessedEventArgs : EventArgs
    {
        public EventProcessedEventArgs(IEvent @event)
        {
            Event = @event;
        }

        public IEvent Event { get; }
    }
}