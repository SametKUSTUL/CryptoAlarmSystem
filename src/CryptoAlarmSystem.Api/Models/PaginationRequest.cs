namespace CryptoAlarmSystem.Api.Models;

/// <summary>
/// Sayfalama parametreleri
/// </summary>
public class PaginationRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Sayfa numarası (1'den başlar)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Sayfa başına kayıt sayısı (Max: 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
