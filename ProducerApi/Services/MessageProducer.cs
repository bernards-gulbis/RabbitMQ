using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProducerApi.Services
{
    public class MessageProducer : IMessageProducer
    {
        private IModel CreateChannel()
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            return connection.CreateModel();
        }

        public void Send<T>(T message, string queue)
        {
            using var channel = CreateChannel();

            channel.QueueDeclare(queue);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish(string.Empty, queue, null, body);
        }
    }
}
