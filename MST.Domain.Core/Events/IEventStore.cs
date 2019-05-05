using System;
using System.Threading.Tasks;

namespace MST.Domain.Abstraction.Events
{
    public interface IEventStore : IDisposable
    {
        Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}