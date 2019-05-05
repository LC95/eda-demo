using System;
using System.Threading.Tasks;
using MST.Domain.Abstraction.Events;
using MST.Domain.Abstraction.Events.DomainEvents;

namespace MST.Domain.Abstraction
{
    public interface IRepository
    {
        Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing;
        Task<TAggregateRoot> GetByIdAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing;
    }
}