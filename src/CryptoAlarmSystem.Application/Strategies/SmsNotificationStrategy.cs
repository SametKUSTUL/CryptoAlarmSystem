using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CryptoAlarmSystem.Application.Strategies;

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<SmsNotificationStrategy> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SmsNotificationStrategy(
        ILogger<SmsNotificationStrategy> logger,
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
            var serviceUrl = _configuration["NotificationServices:Sms:Url"];
            var apiKey = _configuration["NotificationServices:Sms:ApiKey"];

            if (string.IsNullOrEmpty(serviceUrl))
            {
                _logger.LogWarning("SMS service URL not configured. Skipping SMS notification.");
                return;
            }

            var payload = new
            {
                to = $"+1234567890{message.UserId}",
                message = $"Crypto Alarm: {message.CryptoSymbolCode} " +
                          $"{message.AlarmTypeCode} - Target: {message.TargetPrice:F2}, " +
                          $"Current: {message.TriggeredPrice:F2}"
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await httpClient.PostAsJsonAsync(serviceUrl, payload);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "SMS sent successfully: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
                    message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
            }
            else
            {
                _logger.LogError(
                    "Failed to send SMS. Status: {Status}, User={UserId}",
                    response.StatusCode, message.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS notification for User={UserId}", message.UserId);
        }
    }
}
