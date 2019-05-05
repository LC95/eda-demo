using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public interface IAggregateRootWithEventSourcing : IAggregateRoot, IPurgeble, IPersistedVersionSetter
    {
        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        void Replay(IEnumerable<IDomainEvent> events);

        long Version { get; }
    }

    public abstract class AggregateRootWithEventSourcing : IAggregateRootWithEventSourcing
    {
        private readonly Lazy<Dictionary<string, MethodInfo>> _registeredHandlers;
        private readonly Queue<IDomainEvent> uncommittedEvents = new Queue<IDomainEvent>();
        private Guid id;
        private long persistedVersion = 0;
        private object sync = new object();


        #region CTOR

        protected AggregateRootWithEventSourcing() : this(Guid.NewGuid())
        {
        }

        protected AggregateRootWithEventSourcing(Guid id)
        {
            _registeredHandlers = new Lazy<Dictionary<string, MethodInfo>>(() =>
            {
                var methodInfoList = from mi in this.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    let returnType = mi.ReturnType
                    let parameters = mi.GetParameters()
                    where mi.IsDefined(typeof(HandlesInlineAttribute), false) &&
                          returnType == typeof(void) &&
                          parameters.Length == 1 &&
                          typeof(IDomainEvent).IsAssignableFrom(parameters[0].ParameterType)
                    select new {EventName = parameters[0].ParameterType.FullName, MethodInfo = mi};

                return methodInfoList.ToDictionary(methodInfo => methodInfo.EventName,
                    methodInfo => methodInfo.MethodInfo);
            });

            Raise(new AggregateCreatedEvent(id));
        }

        #endregion

        #region PublicProperties

        long IPersistedVersionSetter.PersistedVersion
        {
            set => Interlocked.Exchange(ref this.persistedVersion, value);
        }

        public IEnumerable<IDomainEvent> UncommittedEvents => uncommittedEvents;

        public long Version => this.uncommittedEvents.Count + this.persistedVersion;

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(AggregateRootWithEventSourcing a, AggregateRootWithEventSourcing b) => !(a == b);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(AggregateRootWithEventSourcing a, AggregateRootWithEventSourcing b)
        {
            if ((object) a == null)
            {
                return (object) b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as AggregateRootWithEventSourcing;
            if (other == null)
            {
                return false;
            }

            return this.id.Equals(other.id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => id.GetHashCode();

        void IPurgeble.Purge()
        {
            lock (sync)
            {
                uncommittedEvents.Clear();
            }
        }

        public void Replay(IEnumerable<IDomainEvent> events)
        {
            ((IPurgeble) this).Purge();
            events.OrderBy(e => e.TimeStamp)
                .ToList()
                .ForEach(e =>
                {
                    HandleEvent(e);
                    Interlocked.Increment(ref this.persistedVersion);
                });
        }

        #endregion Public Methods

        #region Protected Methods

        [HandlesInline]
        protected void OnAggregateCreated(AggregateCreatedEvent @event)
        {
            this.id = @event.NewId;
        }

        protected void Raise<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : IDomainEvent
        {
            lock (sync)
            {
                // 首先处理事件数据。
                this.HandleEvent(domainEvent);

                // 然后设置事件的元数据，包括当前事件所对应的聚合根类型以及
                // 聚合的ID值。
                domainEvent.AggregateRootId = id;
                domainEvent.AggregateRootType = GetType().AssemblyQualifiedName;

                domainEvent.Sequence = Version + 1;

                // 最后将事件缓存在“未提交事件”列表中。
                this.uncommittedEvents.Enqueue(domainEvent);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void HandleEvent<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : IDomainEvent
        {
            var key = domainEvent.GetType().FullName;
            if (_registeredHandlers.Value.ContainsKey(key))
            {
                _registeredHandlers.Value[key].Invoke(this, new object[] {domainEvent});
            }
        }

        #endregion Private Methods
    }
}