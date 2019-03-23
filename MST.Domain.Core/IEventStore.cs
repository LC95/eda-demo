using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MST.Domain.Core
{
    public interface IEventStore : IDisposable
    {
        Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
