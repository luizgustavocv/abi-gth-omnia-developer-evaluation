namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Represents the result of cancelling a sale operation.
/// </summary>
/// <remarks>
/// This class is used to return confirmation that a sale was successfully cancelled.
/// It provides a structured response for sale cancellation operations.
/// </remarks>
public class CancelSaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the cancelled sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale number of the cancelled sale.
    /// </summary>
    public long SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the cancellation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was cancelled.
    /// </summary>
    public DateTime CancelledAt { get; set; }
}