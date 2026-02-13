using CryptoAlarmSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAlarmSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PricesController : ControllerBase
{
    private readonly IAlarmCheckService _alarmCheckService;
    private readonly ILogger<PricesController> _logger;

    public PricesController(IAlarmCheckService alarmCheckService, ILogger<PricesController> logger)
    {
        _alarmCheckService = alarmCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Fiyat güncellemesi alır ve alarmları kontrol eder (Worker Service tarafından kullanılır)
    /// </summary>
    /// <param name="request">Kripto sembol ID ve fiyat bilgisi</param>
    /// <returns>Başarılı işlem sonucu</returns>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePrice([FromBody] PriceUpdateRequest request)
    {
        _logger.LogInformation("Received price update: Symbol={SymbolId}, Price={Price}", 
            request.CryptoSymbolId, request.Price);

        await _alarmCheckService.CheckAndTriggerAlarmsAsync(request.CryptoSymbolId, request.Price);

        return Ok();
    }
}

/// <summary>
/// Fiyat güncelleme isteği
/// </summary>
/// <param name="CryptoSymbolId">Kripto para sembol ID (1: BTC, 2: ETH, 3: SOL)</param>
/// <param name="Price">Güncel fiyat</param>
public record PriceUpdateRequest(int CryptoSymbolId, decimal Price);
