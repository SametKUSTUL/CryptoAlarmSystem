namespace CryptoAlarmSystem.Application.DTOs;

public record CreateAlarmRequest(
    int CryptoSymbolId,
    int AlarmTypeId,
    decimal TargetPrice,
    List<int> NotificationChannelIds
);

public record UpdateAlarmChannelsRequest(
    List<int> NotificationChannelIds
);

public record AlarmResponse(
    int Id,
    string UserId,
    int CryptoSymbolId,
    string CryptoSymbolCode,
    string CryptoSymbolName,
    int AlarmTypeId,
    string AlarmTypeCode,
    string AlarmTypeName,
    decimal TargetPrice,
    bool IsTriggered,
    decimal? TriggeredPrice,
    DateTime? TriggeredAt,
    DateTime CreatedAt,
    List<NotificationChannelDto> NotificationChannels
);

public record NotificationChannelDto(
    int Id,
    string Code,
    string Name
);

public record NotificationLogResponse(
    int Id,
    int AlarmId,
    string UserId,
    string CryptoSymbolCode,
    string AlarmTypeCode,
    string NotificationChannelCode,
    decimal TargetPrice,
    decimal TriggeredPrice,
    DateTime SentAt
);

public record CryptoSymbolResponse(
    int Id,
    string Code,
    string Name
);

public record AlarmTypeResponse(
    int Id,
    string Code,
    string Name
);
