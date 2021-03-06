﻿using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MST.EventBus.RabbitMQ
{
    public class RabbitMqEventBus : BaseEventBus
    {
        private readonly bool _autoAck;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly string _exchangeName;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly string _queueName;


        public RabbitMqEventBus(IEventHandlerExecutionContext eventHandlerExecutionContext,
            ILogger<RabbitMqEventBus> logger,
            IConnectionFactory connectionFactory, RabbitMQConfig config) : base(
            eventHandlerExecutionContext)
        {
            _logger = logger;
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = config.RmqExchange;
            var exchangeType = config.RmqExchangeType;
            _queueName = InitializeQueue(config.RmqQueue);
            _autoAck = config.AutoAck;

            _channel.ExchangeDeclare(_exchangeName, exchangeType);
            _logger.LogInformation("RabbitMQEvents构造完成");
        }


        protected override void DisposeObject()
        {
            _channel.Dispose();
            _connection.Dispose();
            _logger.LogInformation($"{GetType().Name} {GetHashCode()} Disposed");
        }

        public override void Subscribe<TEvent, TEventHandler>()
        {
            if (!EventHandlerExecutionContext.IsHandlerRegistered<TEvent, TEventHandler>())
            {
                EventHandlerExecutionContext.RegisterHandler<TEvent, TEventHandler>();
                _channel.QueueBind(_queueName, _exchangeName, typeof(TEvent).FullName);
            }
        }

        /// <summary>
        /// 定义一个Queue, 以及事件接受所需的操作
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private string InitializeQueue(string queue)
        {
            var localQueueName = queue;
            if (string.IsNullOrEmpty(localQueueName))
                localQueueName = _channel.QueueDeclare().QueueName;
            else
                _channel.QueueDeclare(localQueueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArg) =>
            {
                var eventBody = eventArg.Body;
                var json = Encoding.UTF8.GetString(eventBody);
                var @event = (IEvent) JsonConvert.DeserializeObject(json,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
                await EventHandlerExecutionContext.HandleEventAsync(@event);
                if (!_autoAck) _channel.BasicAck(eventArg.DeliveryTag, false);
            };
            _channel.BasicConsume(localQueueName, _autoAck, consumer);
            return localQueueName;
        }


        public override Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(@event,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
            var eventBody = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_exchangeName, @event.GetType().FullName, null, eventBody);
            return Task.CompletedTask;
        }
    }
}