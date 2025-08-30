using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the unique sale number for tracking
    /// </summary>
    public long SaleNumber { get; set; }

    /// <summary>
    /// Gets the date when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the external customer ID (external identity)
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets the customer name (denormalization)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the external branch ID (external identity)
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets the branch name (denormalization)
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the current status of the sale
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the collection of items in this sale
    /// </summary>
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Initializes a new instance of the Sale class
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Confirmed;
        SaleNumber = GenerateSaleNumber();
    }

    /// <summary>
    /// Adds an item to the sale and recalculates the total
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <exception cref="InvalidOperationException">Thrown when sale is cancelled or business rules are violated</exception>
    public void AddItem(SaleItem item)
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot add items to a cancelled sale");
        }

        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        var totalQuantity = existingItem?.Quantity ?? 0 + item.Quantity;

        // Business rule: Maximum 20 items per product
        if (totalQuantity > 20)
        {
            throw new InvalidOperationException("You cannot add more than 20 of the same item to a sale");
        }

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
        else
        {
            item.SaleId = Id;
            Items.Add(item);
        }

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the sale
    /// </summary>
    public void Cancel()
    {
        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        foreach (var item in Items)
        {
            item.Cancel();
        }

        RecalculateTotal();
    }

    /// <summary>
    /// Generates a unique sale number
    /// </summary>
    private long GenerateSaleNumber()
    {
        long min = 1_000_000_000;
        long max = 9_999_999_999;

        return (long)(new Random().NextDouble() * (max - min + 1)) + min;
    }

    /// <summary>
    /// Recalculates the total amount based on items and applies business rules
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
    }

    /// <summary>
    /// Removes an item from the sale and recalculates the total
    /// </summary>
    /// <param name="productId">The ID of the product to remove</param>
    public void RemoveItem(Guid productId)
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot remove items from a cancelled sale");
        }

        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            Items.Remove(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the quantity of an existing item
    /// </summary>
    /// <param name="productId">The ID of the product</param>
    /// <param name="quantity">The new quantity</param>
    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot update items in a cancelled sale");
        }

        if (quantity > 20)
        {
            throw new InvalidOperationException("You cannot add more than 20 of the same item to a sale");
        }

        if (quantity == 0)
        {
            RemoveItem(productId);
            return;
        }

        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            item.UpdateQuantity(quantity);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Validates the sale entity
    /// </summary>
    /// <returns>Validation result with any errors</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}