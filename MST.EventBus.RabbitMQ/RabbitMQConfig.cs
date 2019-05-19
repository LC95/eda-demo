using RabbitMQ.Client;

namespace MST.EventBus.RabbitMQ
{
    public class RabbitMQConfig
    {
        public string RmqExchange { get; set; }
        public string RmqQueue { get; set; }
        public string RmqExchangeType { get; set; } = ExchangeType.Fanout;

        public bool AutoAck { get; set; } = false;
    }
}