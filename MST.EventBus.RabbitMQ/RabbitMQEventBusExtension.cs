using MST.Domain.Abstraction.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;

namespace MST.EventBus.RabbitMQ
{
    public static class RabbitMQEventBusExtension
    {
        public static void AddRabbitMQEventBus(this IServiceCollection services, string exchange, string queue)
        {
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory {HostName = "localhost"});
            services.AddSingleton(new RabbitMQConfig
            {
                RmqExchange = exchange,
                RmqQueue = queue
            });
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
        }
    }
}