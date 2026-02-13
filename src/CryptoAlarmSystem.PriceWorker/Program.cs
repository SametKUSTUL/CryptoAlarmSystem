using CryptoAlarmSystem.PriceWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<PriceGeneratorWorker>();

var host = builder.Build();
host.Run();