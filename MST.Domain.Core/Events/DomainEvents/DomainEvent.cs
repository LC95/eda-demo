using System;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public abstract class DomainEvent : IDomainEvent
    {
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        public Guid AggregateRootId { get; set; }
        public string AggregateRootType { get; set; }
        public long Sequence { get; set; }
    }
}