using System;
using System.Threading.Tasks;

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