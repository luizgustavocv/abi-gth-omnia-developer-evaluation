using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class SaleItemValidator : AbstractValidator<SaleItem>
    {
        public SaleItemValidator()
        {
            RuleFor(item => item.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(item => item.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required")
                .MaximumLength(100)
                .WithMessage("Product name cannot exceed 100 characters");

            RuleFor(item => item.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero");

            RuleFor(item => item.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero")
                .LessThanOrEqualTo(20)
                .WithMessage("You cannot add more than 20 of the same item to a sale");

            RuleFor(item => item.DiscountPercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount percentage cannot be negative")
                .LessThanOrEqualTo(100)
                .WithMessage("Discount percentage cannot exceed 100%");

            RuleFor(item => item.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount amount cannot be negative");

            RuleFor(item => item.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total amount cannot be negative");

            RuleFor(item => item)
                .Must(ValidateDiscountRules)
                .WithMessage("Discount percentage does not match business rules for the given quantity");
        }

        private bool ValidateDiscountRules(SaleItem item)
        {
            if (item.IsCancelled)
            {
                return true;
            }

            var expectedDiscountPercentage = item.Quantity switch
            {
                >= 10 and <= 20 => 20m,
                >= 4 and < 10 => 10m,
                _ => 0m
            };

            return item.DiscountPercentage == expectedDiscountPercentage;
        }
    }
}
