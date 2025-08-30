using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for updating a sale.
/// </summary>
/// <remarks>
/// This command is used to update sale items (add, remove, or modify quantities).
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="UpdateSaleResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="UpdateSaleCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
/// populated and follow the required rules.
/// </remarks>
public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the collection of items to add to the sale
    /// </summary>
    public List<AddSaleItemCommand> ItemsToAdd { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of item updates (quantity changes)
    /// </summary>
    public List<UpdateSaleItemCommand> ItemsToUpdate { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of product IDs to remove from the sale
    /// </summary>
    public List<Guid> ProductIdsToRemove { get; set; } = new();

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateSaleValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}

/// <summary>
/// Command for adding a new item to a sale
/// </summary>
public class AddSaleItemCommand
{
    /// <summary>
    /// Gets or sets the external product ID (external identity)
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name (denormalization)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity of items
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Command for updating an existing item in a sale
/// </summary>
public class UpdateSaleItemCommand
{
    /// <summary>
    /// Gets or sets the product ID to update
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the new quantity
    /// </summary>
    public int Quantity { get; set; }
}