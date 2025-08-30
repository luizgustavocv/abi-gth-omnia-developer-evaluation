namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating a sale
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the collection of items to add to the sale
    /// </summary>
    public List<AddSaleItemRequest> ItemsToAdd { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of item updates (quantity changes)
    /// </summary>
    public List<UpdateSaleItemRequest> ItemsToUpdate { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of product IDs to remove from the sale
    /// </summary>
    public List<Guid> ProductIdsToRemove { get; set; } = new();
}

public class AddSaleItemRequest
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}

public class UpdateSaleItemRequest
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}