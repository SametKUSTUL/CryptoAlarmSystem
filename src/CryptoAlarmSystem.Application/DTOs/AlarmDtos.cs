using CryptoAlarmSystem.Domain.Enums;

namespace CryptoAlarmSystem.Application.DTOs;

public record CreateAlarmRequest(
    int CryptoSymbolId,
    AlarmTypes AlarmTypeId,
    decimal TargetPrice,
    List<NotificationChannels> NotificationChannelIds
);

public record UpdateAlarmChannelsRequest(
    List<NotificationChannels> NotificationChannelIds
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
    string Status,
    decimal? TriggeredPrice,
    DateTime? TriggeredAt,
    DateTime CreatedAt,
    List<NotificationChannelDto> NotificationChannels
);

public record NotificationChannelDto(
    int Id,
    string Code,
    string Name,
    DateTime? SentAt
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
