using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
{
    public CancelSaleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .Length(1, 500)
            .WithMessage("Cancellation reason must contain between 1 and 500 characters");
    }
}