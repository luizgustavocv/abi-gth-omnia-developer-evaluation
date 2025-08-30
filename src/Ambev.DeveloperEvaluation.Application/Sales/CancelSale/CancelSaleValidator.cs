using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Validator for CancelSaleCommand that defines validation rules for sale cancellation.
/// </summary>
public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CancelSaleValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Id: Must not be empty and must be a valid GUID
    /// - CancellationReason: Must not be empty and have reasonable length
    /// </remarks>
    public CancelSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Sale ID cannot be empty");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .Length(1, 500)
            .WithMessage("Cancellation reason must contain between 1 and 500 characters");
    }
}