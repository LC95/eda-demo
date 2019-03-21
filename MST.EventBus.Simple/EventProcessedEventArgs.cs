using System;
using MST.Domain.Core;

namespace MST.EventBus.Simple {
    public class EventProcessedEventArgs : EventArgs {
        public EventProcessedEventArgs(IEvent @event)
        {
            this.Event = @event;
        }
        public IEvent Event { get; }
    }
}
