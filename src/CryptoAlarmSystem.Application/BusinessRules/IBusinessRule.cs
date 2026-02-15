using CryptoAlarmSystem.Domain.Common;

namespace CryptoAlarmSystem.Application.BusinessRules;

public interface IBusinessRule<in TRequest>
{
    Task<Result> ValidateAsync(TRequest request);
}
