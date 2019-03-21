using System;

namespace MST.Domain.Core {
    public interface IEvent {
        Guid Id { get; }
        DateTime TimeStamp { get; }
    }
}
