using CryptoAlarmSystem.Domain.Enums;

namespace CryptoAlarmSystem.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public ErrorCode ErrorCode { get; }
    public string ErrorMessage { get; }

    protected Result(bool isSuccess, ErrorCode errorCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true, ErrorCode.None, string.Empty);
    
    public static Result Failure(ErrorCode errorCode, string errorMessage) => 
        new(false, errorCode, errorMessage);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, ErrorCode errorCode, string errorMessage, T? data) 
        : base(isSuccess, errorCode, errorMessage)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => 
        new(true, ErrorCode.None, string.Empty, data);
    
    public static new Result<T> Failure(ErrorCode errorCode, string errorMessage) => 
        new(false, errorCode, errorMessage, default);
}
