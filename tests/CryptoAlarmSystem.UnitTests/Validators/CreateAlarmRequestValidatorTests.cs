using CryptoAlarmSystem.Api.Validators;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Validators;

public class CreateAlarmRequestValidatorTests
{
    private readonly CreateAlarmRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_ShouldNotHaveValidationError()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidCryptoSymbolId_ShouldHaveValidationError(int symbolId)
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: symbolId,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CryptoSymbolId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidTargetPrice_ShouldHaveValidationError(decimal price)
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: price,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TargetPrice);
    }

    [Fact]
    public void Validate_EmptyNotificationChannels_ShouldHaveValidationError()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels>()
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NotificationChannelIds);
    }

    [Fact]
    public void Validate_MultipleNotificationChannels_ShouldNotHaveValidationError()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Below,
            TargetPrice: 40000m,
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
