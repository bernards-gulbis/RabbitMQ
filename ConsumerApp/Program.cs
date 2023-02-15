using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare("queue_processMessage");

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
};

channel.BasicConsume("queue_processMessage", true, consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();