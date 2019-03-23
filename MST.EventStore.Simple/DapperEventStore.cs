using MST.Domain.Core;
using System;
using System.Threading.Tasks;

namespace MST.EventStore.Simple
{
    public class DapperEventStore : IEventStore
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Console.WriteLine(@event.Id + " " + @event.TimeStamp);
            return Task.CompletedTask;
        }
    }
}
