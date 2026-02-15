namespace CryptoAlarmSystem.Domain.Enums;

public enum ErrorCode
{
    None = 0,
    CryptoSymbolNotFound = 1001,
    DuplicateAlarm = 1002,
    UserIdCannotBeNullOrEmpty=1003
}
