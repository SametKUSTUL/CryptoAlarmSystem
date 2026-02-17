using System.Text;
using System.Text.Json;
using CryptoAlarmSystem.Application.Models;
using CryptoAlarmSystem.Application.Strategies;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CryptoAlarmSystem.NotificationWorker;

public class NotificationConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationConsumer> _logger;
    private IChannel? _channel;
    private const string QueueName = "cryptoalarm.notifications";

    public NotificationConsumer(
        IConnection connection,
        IServiceProvider serviceProvider,
        ILogger<NotificationConsumer> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<NotificationMessage>(json);

                if (message != null)
                {
                    await ProcessNotificationAsync(message);
                    await _channel!.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification");
                await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        _logger.LogInformation("NotificationConsumer started");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessNotificationAsync(NotificationMessage message)
    {
        using var scope = _serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<NotificationStrategyFactory>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var channel in message.Channels)
        {
            var strategy = factory.GetStrategy(channel.Id);
            await strategy.SendAsync(message);
            await LogNotificationAsync(context, message, channel);
        }
    }

    private async Task LogNotificationAsync(AppDbContext context, NotificationMessage message, NotificationChannelInfo channel)
    {
        var log = new NotificationLog
        {
            AlarmId = message.AlarmId,
            NotificationChannelId = channel.Id,
            UserId = message.UserId,
            CryptoSymbolId = message.CryptoSymbolId,
            AlarmTypeId = message.AlarmTypeId,
            TargetPrice = message.TargetPrice,
            TriggeredPrice = message.TriggeredPrice,
            SentAt = DateTime.UtcNow
        };

        context.NotificationLogs.Add(log);
        await context.SaveChangesAsync();
    }
}
