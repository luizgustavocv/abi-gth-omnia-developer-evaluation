using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain event raised when a new sale is created
/// </summary>
public class SaleCreatedEvent
{
    /// <summary>
    /// Gets the ID of the created sale
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
    /// Gets the total amount
    /// </summary>
    public decimal TotalAmount { get; }

    /// <summary>
    /// Gets the number of items in the sale
    /// </summary>
    public int ItemCount { get; }

    /// <summary>
    /// Gets the date when the sale was created
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Initializes a new instance of the SaleCreatedEvent
    /// </summary>
    /// <param name="sale">The created sale</param>
    public SaleCreatedEvent(Sale sale)
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        CustomerId = sale.CustomerId;
        CustomerName = sale.CustomerName;
        BranchId = sale.BranchId;
        BranchName = sale.BranchName;
        TotalAmount = sale.TotalAmount;
        ItemCount = sale.Items.Count;
        CreatedAt = sale.CreatedAt;
    }
}