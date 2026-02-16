using CryptoAlarmSystem.PriceWorker.Models;
using System.Text;
using System.Text.Json;

namespace CryptoAlarmSystem.PriceWorker;

public class PriceGeneratorWorker : BackgroundService
{
    private readonly ILogger<PriceGeneratorWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, (int Id, decimal Min, decimal Max)> _cryptoSymbols = new()
    {
        { "BTC", (1, 40000m, 50000m) },
        { "ETH", (2, 2000m, 3000m) },
        { "SOL", (3, 80m, 120m) },
        { "DOGE", (4, 0.05m, 0.15m) },
        { "LTC", (5, 60m, 100m) },
        { "XRP", (6, 0.40m, 0.60m) },
        { "BNB", (7, 200m, 350m) },
        { "USDT", (8, 0.99m, 1.01m) },
        { "ADA", (9, 0.30m, 0.50m) }
    };

    public PriceGeneratorWorker(
        ILogger<PriceGeneratorWorker> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price Generator Worker started. API is ready.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateAndSendPricesAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Price Generator Worker");
            }
        }
    }

    private async Task GenerateAndSendPricesAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("ApiClient");

        foreach (var (symbol, (id, min, max)) in _cryptoSymbols)
        {
            var price = GenerateRandomPrice(min, max);
            _logger.LogInformation("{Symbol} price: {Price:F2}", symbol, price);

            await SendPriceUpdateAsync(httpClient, id, price);
        }
    }

    private decimal GenerateRandomPrice(decimal min, decimal max)
    {
        var random = new Random();
        var price = (decimal)(random.NextDouble() * (double)(max - min) + (double)min);
        return Math.Round(price, 2);
    }

    private async Task SendPriceUpdateAsync(HttpClient httpClient, int symbolId, decimal price)
    {
        try
        {
            var request = new PriceUpdateRequest(symbolId, price);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/prices/update", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to send price update. Status: {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending price update for symbol {SymbolId}", symbolId);
        }
    }
}
