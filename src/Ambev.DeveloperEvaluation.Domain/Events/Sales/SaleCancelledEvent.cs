using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain event raised when a sale is cancelled
/// </summary>
public class SaleCancelledEvent
{
    /// <summary>
    /// Gets the ID of the cancelled sale
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the sale number
    /// </summary>
    public long SaleNumber { get; }

    /// <summary>
    /// Gets the customer ID
    /// </summary>
    public Guid CustomerId { get; }

    /// <summary>
    /// Gets the customer name
    /// </summary>
    public string CustomerName { get; }

    /// <summary>
    /// Gets the branch ID
    /// </summary>
    public Guid BranchId { get; }

    /// <summary>
    /// Gets the branch name
    /// </summary>
    public string BranchName { get; }

    /// <summary>
    /// Gets the total amount that was cancelled
    /// </summary>
    public decimal TotalAmount { get; }

    /// <summary>
    /// Gets the date when the sale was cancelled
    /// </summary>
    public DateTime CancelledAt { get; }

    /// <summary>
    /// Gets the reason for cancellation
    /// </summary>
    public string CancellationReason { get; }

    /// <summary>
    /// Initializes a new instance of the SaleCancelledEvent
    /// </summary>
    /// <param name="sale">The cancelled sale</param>
    /// <param name="cancellationReason">The reason for cancellation</param>
    public SaleCancelledEvent(Sale sale, string cancellationReason = "Sale cancelled")
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        CustomerId = sale.CustomerId;
        CustomerName = sale.CustomerName;
        BranchId = sale.BranchId;
        BranchName = sale.BranchName;
        TotalAmount = sale.TotalAmount;
        CancelledAt = sale.UpdatedAt ?? DateTime.UtcNow;
        CancellationReason = cancellationReason;
    }
}

/// <summary>
/// Domain event raised when a sale item is cancelled
/// </summary>
public class SaleItemCancelledEvent
{
    /// <summary>
    /// Gets the ID of the cancelled sale item
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the ID of the cancelled sale
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the product ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets the product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of items that was cancelled
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product that was cancelled
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the reason for cancellation
    /// </summary>
    public string CancellationReason { get; }

    /// <summary>
    /// Initializes a new instance of the SaleItemCancelledEvent
    /// </summary>
    /// <param name="saleItem">The cancelled sale item</param>
    /// <param name="cancellationReason">The reason for cancellation</param>
    public SaleItemCancelledEvent(SaleItem saleItem, string cancellationReason = "Sale cancelled")
    {
        Id = saleItem.Id;
        SaleId = saleItem.SaleId;
        ProductId = saleItem.ProductId;
        ProductName = saleItem.ProductName;
        Quantity = saleItem.Quantity;
        UnitPrice = saleItem.UnitPrice;
        CancellationReason = cancellationReason;
    }
}