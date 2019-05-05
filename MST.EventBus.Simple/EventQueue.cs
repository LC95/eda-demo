using System;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.EventBus.Simple
{
    internal sealed class EventQueue
    {
        public event EventHandler<EventProcessedEventArgs> EventPushed;

        public void Push(IEvent @event)
        {
            OnMessagePushed(new EventProcessedEventArgs(@event));
        }

        private void OnMessagePushed(EventProcessedEventArgs e)
        {
            EventPushed?.Invoke(this, e);
        }
    }
}