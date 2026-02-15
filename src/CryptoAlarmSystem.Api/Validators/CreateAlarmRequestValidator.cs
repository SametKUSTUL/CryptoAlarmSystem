using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Enums;
using FluentValidation;

namespace CryptoAlarmSystem.Api.Validators;

public class CreateAlarmRequestValidator : AbstractValidator<CreateAlarmRequest>
{
    public CreateAlarmRequestValidator()
    {
        RuleFor(x => x.CryptoSymbolId)
            .GreaterThan(0).WithMessage("CryptoSymbolId must be greater than 0");

        RuleFor(x => x.AlarmTypeId)
            .IsInEnum().WithMessage("AlarmTypeId must be a valid AlarmType (Above=1, Below=2)");

        RuleFor(x => x.TargetPrice)
            .GreaterThan(0).WithMessage("TargetPrice must be greater than 0");

        RuleFor(x => x.NotificationChannelIds)
            .NotEmpty()
            .WithMessage("At least one notification channel must be selected");
        
        RuleForEach(x => x.NotificationChannelIds)
            .IsInEnum().WithMessage("All notification channel IDs must be valid (Email=1, Sms=2, PushNotification=3)");
    }
}
