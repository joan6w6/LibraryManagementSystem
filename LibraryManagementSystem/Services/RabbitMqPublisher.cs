using System.Text;
using System.Text.Json;
using LibraryManagementSystem.Models;
using RabbitMQ.Client;

public class RabbitMqPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Publish(NotificationMessage message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _configuration["RabbitMQ:QueueName"],
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(
            exchange: "",
            routingKey: _configuration["RabbitMQ:QueueName"],
            basicProperties: null,
            body: body
        );

        Console.WriteLine($"Message published: {message.Subject}");
    }
}
