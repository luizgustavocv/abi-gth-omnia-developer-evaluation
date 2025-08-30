namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Request model for cancelling a sale
/// </summary>
public class CancelSaleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to cancel
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation
    /// </summary>
    public string CancellationReason { get; set; } = "User cancelled the sale";
}