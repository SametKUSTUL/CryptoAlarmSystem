using CryptoAlarmSystem.Application.DTOs;
using FluentValidation;

namespace CryptoAlarmSystem.Api.Validators;

public class UpdateAlarmChannelsRequestValidator : AbstractValidator<UpdateAlarmChannelsRequest>
{
    public UpdateAlarmChannelsRequestValidator()
    {
        RuleFor(x => x.NotificationChannelIds)
            .NotEmpty()
            .WithMessage("At least one notification channel must be selected");
        
        RuleForEach(x => x.NotificationChannelIds)
            .IsInEnum().WithMessage("All notification channel IDs must be valid (Email=1, Sms=2, PushNotification=3)");
    }
}
