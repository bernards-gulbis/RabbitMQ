namespace ProducerApi.Services
{
    public interface IMessageProducer
    {
        public void Send<T>(T message, string queueName, string exchangeName, string routingKey);
    }
}
