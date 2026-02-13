using CryptoAlarmSystem.Application.DTOs;
using FluentValidation;

namespace CryptoAlarmSystem.Api.Validators;

public class CreateAlarmRequestValidator : AbstractValidator<CreateAlarmRequest>
{
    public CreateAlarmRequestValidator()
    {
        RuleFor(x => x.CryptoSymbolId)
            .GreaterThan(0).WithMessage("CryptoSymbolId must be greater than 0");

        RuleFor(x => x.AlarmTypeId)
            .GreaterThan(0).WithMessage("AlarmTypeId must be greater than 0");

        RuleFor(x => x.TargetPrice)
            .GreaterThan(0).WithMessage("TargetPrice must be greater than 0");

        RuleFor(x => x.NotificationChannelIds)
            .NotEmpty().WithMessage("At least one notification channel is required")
            .Must(x => x.All(id => id > 0)).WithMessage("All notification channel IDs must be greater than 0");
    }
}
