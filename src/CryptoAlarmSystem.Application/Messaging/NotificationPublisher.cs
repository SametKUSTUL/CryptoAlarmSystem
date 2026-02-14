using System.Text;
using System.Text.Json;
using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CryptoAlarmSystem.Application.Messaging;

public class NotificationPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<NotificationPublisher> _logger;
    private const string QueueName = "notifications";

    public NotificationPublisher(IConnection connection, ILogger<NotificationPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync(NotificationMessage message)
    {
        var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: "", routingKey: QueueName, mandatory: false, basicProperties: properties, body: body);
        
        _logger.LogInformation("Published notification to queue: AlarmId={AlarmId}", message.AlarmId);
    }
}
