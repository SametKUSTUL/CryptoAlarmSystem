using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CryptoAlarmSystem.Application.Strategies;

public class PushNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<PushNotificationStrategy> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PushNotificationStrategy(
        ILogger<PushNotificationStrategy> logger,
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
            var serviceUrl = _configuration["NotificationServices:Push:Url"];
            var apiKey = _configuration["NotificationServices:Push:ApiKey"];

            if (string.IsNullOrEmpty(serviceUrl))
            {
                _logger.LogWarning("Push service URL not configured. Skipping push notification.");
                return;
            }

            var payload = new
            {
                userId = message.UserId,
                title = $"Crypto Alarm: {message.CryptoSymbolCode}",
                body = $"Your alarm has been triggered!\n" +
                       $"Type: {message.AlarmTypeCode}\n" +
                       $"Target: {message.TargetPrice:F2}, Current: {message.TriggeredPrice:F2}",
                data = new
                {
                    symbolCode = message.CryptoSymbolCode,
                    alarmType = message.AlarmTypeCode,
                    targetPrice = message.TargetPrice,
                    triggeredPrice = message.TriggeredPrice
                }
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await httpClient.PostAsJsonAsync(serviceUrl, payload);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "PUSH sent successfully: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
                    message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
            }
            else
            {
                _logger.LogError(
                    "Failed to send push notification. Status: {Status}, User={UserId}",
                    response.StatusCode, message.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification for User={UserId}", message.UserId);
        }
    }
}
