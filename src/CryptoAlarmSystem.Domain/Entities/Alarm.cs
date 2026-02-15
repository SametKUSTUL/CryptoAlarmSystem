namespace CryptoAlarmSystem.Domain.Entities;

public class Alarm
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int CryptoSymbolId { get; set; }
    public int AlarmTypeId { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal? TriggeredPrice { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Enums.AlarmStatus Status { get; set; }
    
    public CryptoSymbol CryptoSymbol { get; set; } = null!;
    public AlarmType AlarmType { get; set; } = null!;
    public ICollection<AlarmNotificationChannel> AlarmNotificationChannels { get; set; } = new List<AlarmNotificationChannel>();
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
