using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CryptoAlarmSystem.Application.Strategies;

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<EmailNotificationStrategy> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public EmailNotificationStrategy(
        ILogger<EmailNotificationStrategy> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task SendAsync(NotificationMessage message)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var serviceUrl = _configuration["NotificationServices:Email:Url"];
            var apiKey = _configuration["NotificationServices:Email:ApiKey"];

            if (string.IsNullOrEmpty(serviceUrl))
            {
                _logger.LogWarning("Email service URL not configured. Skipping email notification.");
                return;
            }

            var payload = new
            {
                to = $"{message.UserId}@example.com",
                subject = $"Crypto Alarm: {message.CryptoSymbolCode}",
                body = $"Your alarm has been triggered!\n\n" +
                       $"Symbol: {message.CryptoSymbolCode}\n" +
                       $"Alarm Type: {message.AlarmTypeCode}\n" +
                       $"Target Price: {message.TargetPrice:F2}\n" +
                       $"Current Price: {message.TriggeredPrice:F2}"
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await httpClient.PostAsJsonAsync(serviceUrl, payload);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "EMAIL sent successfully: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
                    message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
            }
            else
            {
                _logger.LogError(
                    "Failed to send email. Status: {Status}, User={UserId}",
                    response.StatusCode, message.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification for User={UserId}", message.UserId);
        }
    }
}
