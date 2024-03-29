﻿using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProducerApi.Services
{
    internal class MessageProducer : IMessageProducer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger<MessageProducer> _logger;

        public MessageProducer(ILogger<MessageProducer> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;

            var factory = new ConnectionFactory
            {
                ClientProvidedName = "Producer Api",
                UserName = Environment.GetEnvironmentVariable("RMQ_USER"),
                Password = Environment.GetEnvironmentVariable("RMQ_PASS"),
                HostName = Environment.GetEnvironmentVariable("RMQ_HOST"),
                Port = Int32.Parse(Environment.GetEnvironmentVariable("RMQ_PORT"))
            };

            _logger.LogDebug("Creating connection");
            _connection = factory.CreateConnection();
        }

        public void Send<T>(T message, string queueName, string exchangeName, string routingKey)
        {
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            _logger.LogDebug("Publishing");
            channel.BasicPublish(exchangeName, routingKey, null, body);
        }

        public void Dispose()
        {
            _logger.LogDebug("Closing connection");
            _connection.Close();
        }
    }
}
