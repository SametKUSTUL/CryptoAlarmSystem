using CryptoAlarmSystem.Application.BusinessRules;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;

namespace CryptoAlarmSystem.Application.Workflows;

public class CreateAlarmWorkflow : IWorkflow<CreateAlarmRequest>
{
    private readonly IEnumerable<IBusinessRule<CreateAlarmRequest>> _rules;

    public CreateAlarmWorkflow(IEnumerable<IBusinessRule<CreateAlarmRequest>> rules)
    {
        _rules = rules;
    }

    public async Task<Result> ExecuteAsync(CreateAlarmRequest request)
    {
        foreach (var rule in _rules)
        {
            var result = await rule.ValidateAsync(request);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.Success();
    }
}
