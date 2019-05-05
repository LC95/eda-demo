using System;

namespace MST.Domain.Abstraction.Events
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime TimeStamp { get; }
    }
}