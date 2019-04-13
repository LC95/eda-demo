using System;

namespace MST.Domain.Abstraction
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime TimeStamp { get; }
    }
}