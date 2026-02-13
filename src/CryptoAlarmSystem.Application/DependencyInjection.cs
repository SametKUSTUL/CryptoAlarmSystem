using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoAlarmSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Scoped services
        services.AddScoped<IAlarmService, AlarmService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Hosted service (Singleton)
        services.AddHostedService<PriceMonitorBackgroundService>();
        
        return services;
    }
}
