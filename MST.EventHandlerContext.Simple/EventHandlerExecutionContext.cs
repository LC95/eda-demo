using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;

namespace MST.EventHandlerContext.Simple
{
    public class EventHandlerExecutionContext : IEventHandlerExecutionContext
    {
        private readonly ConcurrentDictionary<Type, List<Type>> _registrations =
            new ConcurrentDictionary<Type, List<Type>>();

        private readonly IServiceCollection _registry;
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFactory;

        public EventHandlerExecutionContext(IServiceCollection registry,
            Func<IServiceCollection, IServiceProvider> serviceProviderFactory)
        {
            _registry = registry;
            _serviceProviderFactory = serviceProviderFactory;
        }

        /// <summary>
        ///     注册处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        public void RegisterHandler<TEvent, THandler>() where TEvent : IEvent where THandler : IEventHandler<TEvent>
        {
            RegisterHandler(typeof(TEvent), typeof(THandler));
        }

        /// <summary>
        ///     注册处理器(泛型)
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handlerType"></param>
        public void RegisterHandler(Type eventType, Type handlerType)
        {
            ConcurrentDictionarySafeRegister(eventType, handlerType, _registrations);
            _registry.AddTransient(handlerType);
        }

        /// <summary>
        ///     判断事件是否有对应的处理器
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public bool HandlerRegistered(Type eventType, Type handlerType)
        {
            if (_registrations.TryGetValue(eventType, out var handlerTypes))
                return handlerTypes != null && handlerTypes.Contains(handlerType);

            return false;
        }

        /// <summary>
        ///     判断事件是否有对应的处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public bool HandlerRegistered<TEvent, THandler>() where TEvent : IEvent where THandler : IEventHandler<TEvent>
        {
            return HandlerRegistered(typeof(TEvent), typeof(THandler));
        }

        /// <summary>
        ///     处理事件
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            var eventType = @event.GetType();
            if (_registrations.TryGetValue(eventType, out var handlerTypes) && handlerTypes?.Count > 0)
            {
                var serviceProvider = _serviceProviderFactory(_registry);
                using (var childScope = serviceProvider.CreateScope()) //处理器作用于此生命周期, 这也是该类的存在的核心
                {
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = (IEventHandler) childScope.ServiceProvider.GetService(handlerType);
                        if (handler.CanHandle(@event)) await handler.HandleAsync(@event, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        ///     一个维护事件与事件处理器的对应关系的并发字典
        ///     可以在
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventHandler"></param>
        /// <param name="registry"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        private static void ConcurrentDictionarySafeRegister<TEvent, TEventHandler>(TEvent @event,
            TEventHandler eventHandler,
            ConcurrentDictionary<TEvent, List<TEventHandler>> registry)
        {
            if (registry.TryGetValue(@event, out var registryItem))
            {
                //如果该事件有处理器, 但是处理器中没有该条处理器, 就增加一条处理器
                if (registryItem != null)
                {
                    if (!registryItem.Contains(eventHandler)) registry[@event].Add(eventHandler);
                }
                else
                {
                    registry[@event] = new List<TEventHandler> {eventHandler};
                }
            }
            else
            {
                //如果该事件没有处理器, 加一条处理器
                registry.TryAdd(@event, new List<TEventHandler> {eventHandler});
            }
        }
    }
}