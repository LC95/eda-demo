using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MST.Domain.Abstraction.Events.DomainEvents;

namespace MST.Domain.Abstraction
{
    public abstract class Repository
    {
        public Repository()
        {
        }

        public async Task<TAggregateRoot> GetByIdAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing
        {
            var domainEvents = await LoadDomainEventsAsync(typeof(TAggregateRoot), id);
            var aggregateRoot = ActivateAggregateRoot<TAggregateRoot>();
            aggregateRoot.Replay(domainEvents);
            return aggregateRoot;
        }

        public async Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing
        {
            var domainEvents = aggregateRoot.UncommittedEvents;
            await this.PersistDomainEventsAsync(domainEvents);
            aggregateRoot.PersistedVersion = aggregateRoot.Version;
            aggregateRoot.Purge();
        }

        protected abstract Task<IEnumerable<IDomainEvent>> LoadDomainEventsAsync(Type aggregateRootType, Guid id);

        protected abstract Task PersistDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents);

        private TAggregateRoot ActivateAggregateRoot<TAggregateRoot>()
            where TAggregateRoot : class, IAggregateRootWithEventSourcing
        {
            var constructors = from ctor in typeof(TAggregateRoot).GetTypeInfo().GetConstructors()
                let parameters = ctor.GetParameters()
                where parameters.Length == 0 ||
                      parameters.Length == 1 && parameters[0].ParameterType == typeof(Guid)
                select new {ConstructorInfo = ctor, ParameterCount = parameters.Length};

            if (constructors.Any())
            {
                TAggregateRoot aggregateRoot;
                var constructorDefinition = constructors.First();
                if (constructorDefinition.ParameterCount == 0)
                {
                    aggregateRoot = (TAggregateRoot) constructorDefinition.ConstructorInfo.Invoke(null);
                }
                else
                {
                    aggregateRoot =
                        (TAggregateRoot) constructorDefinition.ConstructorInfo.Invoke(new object[] {Guid.NewGuid()});
                }

                // 将AggregateRoot下的所有事件清除。事实上，在AggregateRoot的构造函数中，已经产生了AggregateCreatedEvent。
                aggregateRoot.Purge();
                return aggregateRoot;
            }

            return null;
        }
    }
}