namespace CryptoAlarmSystem.Application.Models;

public record NotificationMessage(
    int AlarmId,
    string UserId,
    string CryptoSymbolCode,
    string CryptoSymbolName,
    int CryptoSymbolId,
    string AlarmTypeCode,
    int AlarmTypeId,
    decimal TargetPrice,
    decimal TriggeredPrice,
    List<NotificationChannelInfo> Channels
);

public record NotificationChannelInfo(
    int Id,
    string Code,
    string Name
);
