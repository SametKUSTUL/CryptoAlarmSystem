namespace CryptoAlarmSystem.Domain.Entities;

public class AlarmNotificationChannel
{
    public int Id { get; set; }
    public int AlarmId { get; set; }
    public int NotificationChannelId { get; set; }
    
    public Alarm Alarm { get; set; } = null!;
    public NotificationChannel NotificationChannel { get; set; } = null!;
}
