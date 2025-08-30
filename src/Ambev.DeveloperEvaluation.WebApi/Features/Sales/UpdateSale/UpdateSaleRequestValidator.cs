using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleForEach(x => x.ItemsToAdd)
            .SetValidator(new AddSaleItemRequestValidator());

        RuleForEach(x => x.ItemsToUpdate)
            .SetValidator(new UpdateSaleItemRequestValidator());
    }
}

public class AddSaleItemRequestValidator : AbstractValidator<AddSaleItemRequest>
{
    public AddSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than 0");
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20).WithMessage("Quantity must contain between 1 and 20");
    }
}

public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required");
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20).WithMessage("Quantity must contain between 1 and 20");
    }
}