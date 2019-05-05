using System;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public sealed class AggregateCreatedEvent : DomainEvent
    {
        public AggregateCreatedEvent(Guid newId)
        {
            NewId = newId;
        }

        public Guid NewId { get; set; }
    }
}