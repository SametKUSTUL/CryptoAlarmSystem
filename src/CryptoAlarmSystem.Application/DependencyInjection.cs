using CryptoAlarmSystem.Application.BusinessRules;
using CryptoAlarmSystem.Application.BusinessRules.CreateAlarm;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Application.Messaging;
using CryptoAlarmSystem.Application.Services;
using CryptoAlarmSystem.Application.Strategies;
using CryptoAlarmSystem.Application.Workflows;
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
        
        // Business Rules for CreateAlarm
        services.AddScoped<IBusinessRule<CreateAlarmRequest>, CryptoSymbolExistsRule>();
        services.AddScoped <IBusinessRule<CreateAlarmRequest>, NoDuplicateActiveAlarmRule>();
        
        // Workflows
        services.AddScoped<IWorkflow<CreateAlarmRequest>, CreateAlarmWorkflow>();
        
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory { HostName = rabbitMqHost };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
        services.AddSingleton<NotificationPublisher>();
        
        return services;
    }
}
