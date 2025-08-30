using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Response model for sale creation
/// </summary>
public class CreateSaleResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the created sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique sale number
    /// </summary>
    public long SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was created
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch ID
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch name
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the status of the sale
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of sale items
    /// </summary>
    public List<CreateSaleItemResponse> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the date when the sale was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response model for a sale item within a created sale
/// </summary>
public class CreateSaleItemResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item
    /// </summary>
    public decimal TotalAmount { get; set; }
}