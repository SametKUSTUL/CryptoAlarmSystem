namespace CryptoAlarmSystem.Domain.Entities;

public class NotificationLog
{
    public int Id { get; set; }
    public int AlarmId { get; set; }
    public int NotificationChannelId { get; set; }
    public string UserId { get; set; } = null!;
    public int CryptoSymbolId { get; set; }
    public int AlarmTypeId { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal TriggeredPrice { get; set; }
    public DateTime SentAt { get; set; }
    
    public Alarm Alarm { get; set; } = null!;
    public NotificationChannel NotificationChannel { get; set; } = null!;
    public CryptoSymbol CryptoSymbol { get; set; } = null!;
    public AlarmType AlarmType { get; set; } = null!;
}
