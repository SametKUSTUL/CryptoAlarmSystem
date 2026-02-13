using CryptoAlarmSystem.Application.DTOs;
using FluentValidation;

namespace CryptoAlarmSystem.Api.Validators;

public class UpdateAlarmChannelsRequestValidator : AbstractValidator<UpdateAlarmChannelsRequest>
{
    public UpdateAlarmChannelsRequestValidator()
    {
        RuleFor(x => x.NotificationChannelIds)
            .NotEmpty().WithMessage("At least one notification channel is required")
            .Must(x => x.All(id => id > 0)).WithMessage("All notification channel IDs must be greater than 0");
    }
}
