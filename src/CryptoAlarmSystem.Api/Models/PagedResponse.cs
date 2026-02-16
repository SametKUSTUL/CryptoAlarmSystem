namespace CryptoAlarmSystem.Api.Models;

/// <summary>
/// Sayfalanmış veri yanıtı
/// </summary>
public class PagedResponse<T>
{
    /// <summary>
    /// Veri listesi
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Mevcut sayfa numarası
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Sayfa başına kayıt sayısı
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Toplam kayıt sayısı
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Bir sonraki sayfa var mı?
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Bir önceki sayfa var mı?
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }
}
