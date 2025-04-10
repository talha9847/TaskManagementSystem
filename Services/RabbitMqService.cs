using RabbitMQ.Client;
using System;
using System.Text;

namespace MyApp.Namespace
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };  
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "task_events", type: ExchangeType.Fanout);
        }

        public void SendNotificationEvent(Notification notification)
        {
            var message = notification.ToString();
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "task_events", routingKey: "", basicProperties: null, body: body);
        }
    }
}
