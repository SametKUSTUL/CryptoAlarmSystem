using CryptoAlarmSystem.Application.Strategies;
using CryptoAlarmSystem.Infrastructure;
using CryptoAlarmSystem.NotificationWorker;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory { HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost" };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddTransient<EmailNotificationStrategy>();
builder.Services.AddTransient<SmsNotificationStrategy>();
builder.Services.AddTransient<PushNotificationStrategy>();
builder.Services.AddTransient<NotificationStrategyFactory>();

builder.Services.AddHostedService<NotificationConsumer>();

var host = builder.Build();
host.Run();
