using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory
{
    ClientProvidedName = "Consumer App",
    UserName = Environment.GetEnvironmentVariable("RMQ_USER"),
    Password = Environment.GetEnvironmentVariable("RMQ_PASS"),
    HostName = Environment.GetEnvironmentVariable("RMQ_HOST"),
    Port = Int32.Parse(Environment.GetEnvironmentVariable("RMQ_PORT"))
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "DemoExchange";
var routingKey = "demo-routing-key";
var queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received: {message}");

    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

channel.BasicCancel(consumerTag);