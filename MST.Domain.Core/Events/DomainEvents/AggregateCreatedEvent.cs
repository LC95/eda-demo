using System;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public sealed class AggregateCreatedEvent : DomainEvent
    {
        public AggregateCreatedEvent(Guid newId)
        {
            this.NewId = newId;
        }

        public Guid NewId { get; set; }
    }
}