using System;
using MST.Domain.Abstraction.Events;

namespace MST.EventBus.Simple
{
    /// <summary>
    /// EventHandler代理
    /// </summary>
    internal sealed class EventQueue
    {
        public event System.EventHandler<EventProcessedEventArgs> EventPushed;

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