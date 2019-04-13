using System;
using MST.Domain.Abstraction;

namespace MST.Domain
{
    public class CustomerCreatedEvent : IEvent
    {
        public CustomerCreatedEvent(string customerName)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            CustomerName = customerName;
        }

        public string CustomerName { get; }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
    }
}