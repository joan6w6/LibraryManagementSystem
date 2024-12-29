using System.Text;
using System.Text.Json;
using LibraryManagementSystem.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(IConfiguration configuration, EmailService emailService, ILogger<RabbitMqConsumer> logger)
    {
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<NotificationMessage>(messageJson);

                    if (message != null)
                    {
                        _logger.LogInformation($"接收到消息: {message.Subject}");
                        await _emailService.SendEmailAsync(message.Email, message.Subject, message.Body);
                        _logger.LogInformation($"邮件已发送到: {message.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"处理消息时发生错误: {ex.Message}");
                }
            };

            channel.BasicConsume(
                queue: _configuration["RabbitMQ:QueueName"],
                autoAck: true,
                consumer: consumer
            );

            _logger.LogInformation("RabbitMQ Consumer 已启动");
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(1000); // 保持服务运行
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }
}
