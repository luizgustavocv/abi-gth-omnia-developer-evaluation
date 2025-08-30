using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain event raised when a sale is modified
/// </summary>
public class SaleModifiedEvent
{
    /// <summary>
    /// Gets the ID of the modified sale
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
    /// Gets the total amount
    /// </summary>
    public decimal TotalAmount { get; }

    /// <summary>
    /// Gets the number of items in the sale
    /// </summary>
    public int ItemCount { get; }

    /// <summary>
    /// Gets the date when the sale was modified
    /// </summary>
    public DateTime ModifiedAt { get; }

    /// <summary>
    /// Gets the description of what was modified
    /// </summary>
    public string ModificationType { get; }

    /// <summary>
    /// Initializes a new instance of the SaleModifiedEvent
    /// </summary>
    /// <param name="sale">The modified sale</param>
    /// <param name="modificationType">Description of the modification</param>
    public SaleModifiedEvent(Sale sale, string modificationType)
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        CustomerId = sale.CustomerId;
        CustomerName = sale.CustomerName;
        TotalAmount = sale.TotalAmount;
        ItemCount = sale.Items.Count;
        ModifiedAt = sale.UpdatedAt ?? DateTime.UtcNow;
        ModificationType = modificationType;
    }
}