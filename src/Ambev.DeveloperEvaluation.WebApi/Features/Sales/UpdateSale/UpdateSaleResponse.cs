using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Response model for updating a sale
/// </summary>
public class UpdateSaleResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the updated sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique sale number
    /// </summary>
    public long SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the sale after update
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the status of the sale
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the number of items in the sale
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a message describing the update result
    /// </summary>
    public string Message { get; set; } = string.Empty;
}