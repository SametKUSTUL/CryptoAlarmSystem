using CryptoAlarmSystem.Domain.Common;

namespace CryptoAlarmSystem.Application.Workflows;

public interface IWorkflow<in TRequest>
{
    Task<Result> ExecuteAsync(TRequest request);
}
