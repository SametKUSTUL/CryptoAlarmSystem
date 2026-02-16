using CryptoAlarmSystem.Api.Controllers.V1;
using FluentValidation;

namespace CryptoAlarmSystem.Api.Validators;

public class PriceUpdateRequestValidator : AbstractValidator<PriceUpdateRequest>
{
    public PriceUpdateRequestValidator()
    {
        RuleFor(x => x.CryptoSymbolId)
            .GreaterThan(0).WithMessage("CryptoSymbolId must be greater than 0");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
