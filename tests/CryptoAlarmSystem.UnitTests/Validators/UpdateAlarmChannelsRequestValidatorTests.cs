using CryptoAlarmSystem.Api.Validators;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Validators;

public class UpdateAlarmChannelsRequestValidatorTests
{
    private readonly UpdateAlarmChannelsRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_ShouldNotHaveValidationError()
    {
        var request = new UpdateAlarmChannelsRequest(
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyChannels_ShouldHaveValidationError()
    {
        var request = new UpdateAlarmChannelsRequest(
            NotificationChannelIds: new List<NotificationChannels>()
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NotificationChannelIds);
    }

    [Fact]
    public void Validate_MultipleChannels_ShouldNotHaveValidationError()
    {
        var request = new UpdateAlarmChannelsRequest(
            NotificationChannelIds: new List<NotificationChannels>
            {
                NotificationChannels.Email,
                NotificationChannels.Sms,
                NotificationChannels.PushNotification
            }
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
