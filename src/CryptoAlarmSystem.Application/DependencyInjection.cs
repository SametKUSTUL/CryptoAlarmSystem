using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Application.Messaging;
using CryptoAlarmSystem.Application.Services;
using CryptoAlarmSystem.Application.Strategies;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CryptoAlarmSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, string rabbitMqHost = "localhost")
    {
        services.AddScoped<IAlarmService, AlarmService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAlarmCheckService, AlarmCheckService>();
        
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory { HostName = rabbitMqHost };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
        services.AddSingleton<NotificationPublisher>();
        
        return services;
    }
}
