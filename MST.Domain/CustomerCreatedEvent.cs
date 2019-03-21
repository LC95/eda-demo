using System;
using MST.Domain.Core;

namespace MST.Domain {
    public class CustomerCreatedEvent : IEvent {
        public CustomerCreatedEvent(string customerName)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            CustomerName = customerName;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        public string CustomerName { get;}
    }
}
