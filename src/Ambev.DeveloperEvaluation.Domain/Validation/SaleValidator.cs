using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class SaleValidator : AbstractValidator<Sale>
    {
        public SaleValidator()
        {
            RuleFor(sale => sale.SaleNumber)
                .NotEmpty()
                .WithMessage("Sale number is required");

            RuleFor(sale => sale.SaleDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Sale date cannot be in the future");

            RuleFor(sale => sale.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(sale => sale.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required")
                .MaximumLength(100)
                .WithMessage("Customer name cannot exceed 100 characters");

            RuleFor(sale => sale.BranchId)
                .NotEmpty()
                .WithMessage("Branch ID is required");

            RuleFor(sale => sale.BranchName)
                .NotEmpty()
                .WithMessage("Branch name is required")
                .MaximumLength(100)
                .WithMessage("Branch name cannot exceed 100 characters");

            RuleFor(sale => sale.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total amount cannot be negative");

            RuleFor(sale => sale.Status)
                .IsInEnum()
                .WithMessage("Sale status must be a valid value");

            RuleFor(sale => sale.Items)
                .NotNull()
                .WithMessage("Items collection is required");

            RuleFor(sale => sale.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Created date cannot be in the future");

            RuleForEach(sale => sale.Items)
                .SetValidator(new SaleItemValidator())
                .When(sale => sale.Items != null);

            RuleFor(sale => sale)
                .Must(ValidateUniqueProducts)
                .WithMessage("Sale cannot contain duplicate products")
                .When(sale => sale.Items != null && sale.Items.Any());

            RuleFor(sale => sale)
                .Must(ValidateTotalAmountCalculation)
                .WithMessage("Total amount does not match the sum of item totals")
                .When(sale => sale.Items != null);
        }

        private bool ValidateUniqueProducts(Sale sale)
        {
            if (sale.Items == null || !sale.Items.Any())
            {
                return true;
            }

            var productIds = sale.Items.Select(item => item.ProductId).ToList();

            return productIds.Count == productIds.Distinct().Count();
        }

        private bool ValidateTotalAmountCalculation(Sale sale)
        {
            if (sale.Items == null || !sale.Items.Any())
            {
                return sale.TotalAmount == 0;
            }

            var calculatedTotal = sale.Items.Sum(item => item.TotalAmount);

            return Math.Abs(sale.TotalAmount - calculatedTotal) < 0.01m;
        }
    }
}
