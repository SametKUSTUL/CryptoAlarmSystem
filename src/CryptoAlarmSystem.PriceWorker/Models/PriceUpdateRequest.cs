namespace CryptoAlarmSystem.PriceWorker.Models;

public record PriceUpdateRequest(
    int CryptoSymbolId,
    decimal Price
);
