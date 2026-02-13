namespace CryptoAlarmSystem.Domain.Entities;

public class NotificationChannel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public ICollection<AlarmNotificationChannel> AlarmNotificationChannels { get; set; } = new List<AlarmNotificationChannel>();
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
