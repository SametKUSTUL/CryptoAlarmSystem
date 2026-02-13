namespace CryptoAlarmSystem.Domain.Entities;

public class AlarmType
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public ICollection<Alarm> Alarms { get; set; } = new List<Alarm>();
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
