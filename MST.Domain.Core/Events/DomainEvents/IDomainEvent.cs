using System;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public interface IDomainEvent : IEvent
    {
        Guid AggregateRootId { get; set; }
        string AggregateRootType { get; set; }
        long Sequence { get; set; }
    }
}