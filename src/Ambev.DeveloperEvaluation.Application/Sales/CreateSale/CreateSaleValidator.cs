using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation.
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - CustomerId: Must not be empty
    /// - CustomerName: Must not be empty and have reasonable length
    /// - BranchId: Must not be empty
    /// - BranchName: Must not be empty and have reasonable length
    /// - Items: Must have at least one item and each item must be valid
    /// </remarks>
    public CreateSaleValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .Length(1, 100)
            .WithMessage("Customer name must contain between 1 and 100 characters");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required")
            .Length(1, 100)
            .WithMessage("Branch name must contain between 1 and 100 characters");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("A sale must contain at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemCommand that defines validation rules for sale item creation.
/// </summary>
public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleItemCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - ProductId: Must not be empty
    /// - ProductName: Must not be empty and have reasonable length
    /// - UnitPrice: Must be greater than 0
    /// - Quantity: must contain between 1 and 20 (business rule)
    /// </remarks>
    public CreateSaleItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .Length(1, 100)
            .WithMessage("Product name must contain between 1 and 100 characters");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20)
            .WithMessage("You cannot add more than 20 of the same item to a sale");
    }
}