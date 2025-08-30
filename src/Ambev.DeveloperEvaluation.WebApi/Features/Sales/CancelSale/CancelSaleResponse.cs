namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Response model for cancelling a sale
/// </summary>
public class CancelSaleResponse
{
    /// <summary>
    /// Gets or sets whether the cancellation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the cancelled sale
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale number of the cancelled sale
    /// </summary>
    public long SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was cancelled
    /// </summary>
    public DateTime CancelledAt { get; set; }
}