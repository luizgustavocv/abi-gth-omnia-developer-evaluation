using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover discount calculation, business rules, quantity updates, and validation scenarios.
/// </summary>
public class SaleItemTests
{
    [Fact(DisplayName = "Cancelled item should not apply business rules")]
    public void Given_CancelledSaleItem_When_ApplyingBusinessRules_Then_ShouldNotApplyDiscount()
    {
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), 10m, 10);

        saleItem.DiscountPercentage.Should().Be(20);

        saleItem.Cancel();

        saleItem.DiscountPercentage.Should().Be(0);
        saleItem.DiscountAmount.Should().Be(0);
        saleItem.TotalAmount.Should().Be(0);
    }

    [Fact(DisplayName = "UpdateQuantity should throw exception for cancelled item")]
    public void Given_CancelledSaleItem_When_UpdatingQuantity_Then_ShouldThrowException()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();

        saleItem.Cancel();

        var act = () => saleItem.UpdateQuantity(5);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update quantity of a cancelled item");
    }

    [Fact(DisplayName = "UpdateUnitPrice should throw exception for cancelled item")]
    public void Given_CancelledSaleItem_When_UpdatingUnitPrice_Then_ShouldThrowException()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();

        saleItem.Cancel();

        var act = () => saleItem.UpdateUnitPrice(15m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update price of a cancelled item");
    }

    [Theory(DisplayName = "SaleItem should apply correct discount based on quantity")]
    [InlineData(3, 0, 0)]
    [InlineData(4, 10, 4)]
    [InlineData(9, 10, 9)]
    [InlineData(10, 20, 20)]
    [InlineData(20, 20, 40)]
    public void Given_DifferentQuantities_When_CreatingSaleItem_Then_ShouldApplyCorrectDiscount(int quantity, decimal expectedDiscountPercentage, decimal expectedDiscountAmount)
    {
        var unitPrice = 10m;
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), unitPrice, quantity);

        saleItem.DiscountPercentage.Should().Be(expectedDiscountPercentage);
        saleItem.DiscountAmount.Should().Be(expectedDiscountAmount);

        var expectedSubtotal = unitPrice * quantity;
        var expectedTotal = expectedSubtotal - expectedDiscountAmount;

        saleItem.TotalAmount.Should().Be(expectedTotal);
    }

    [Theory(DisplayName = "Business rules should be applied correctly for edge cases")]
    [InlineData(4, 10)]
    [InlineData(9, 10)]
    [InlineData(10, 20)]
    [InlineData(20, 20)]
    public void Given_EdgeCaseQuantities_When_CreatingSaleItem_Then_ShouldApplyCorrectDiscount(int quantity, decimal expectedDiscountPercentage)
    {
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), 10m, quantity);

        saleItem.DiscountPercentage.Should().Be(expectedDiscountPercentage);
    }

    [Fact(DisplayName = "Cancel should set item as cancelled and reset amounts")]
    public void Given_SaleItem_When_Cancelled_Then_ShouldSetCancelledAndResetAmounts()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();

        saleItem.TotalAmount.Should().BeGreaterThan(0);

        saleItem.Cancel();

        saleItem.IsCancelled.Should().BeTrue();
        saleItem.TotalAmount.Should().Be(0);
        saleItem.DiscountAmount.Should().Be(0);
        saleItem.DiscountPercentage.Should().Be(0);
    }

    [Fact(DisplayName = "UpdateQuantity should recalculate discount and total")]
    public void Given_SaleItem_When_UpdatingQuantity_Then_ShouldRecalculateDiscountAndTotal()
    {
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), 10m, 2);

        saleItem.TotalAmount.Should().Be(20m);
        saleItem.UpdateQuantity(4);

        saleItem.DiscountAmount.Should().Be(4m);
        saleItem.DiscountPercentage.Should().Be(10);
        saleItem.Quantity.Should().Be(4);
        saleItem.TotalAmount.Should().Be(36m);
    }

    [Fact(DisplayName = "Multiple quantity updates should maintain business rules")]
    public void Given_SaleItem_When_UpdatingQuantityMultipleTimes_Then_ShouldMaintainBusinessRules()
    {
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), 10m, 1);

        saleItem.DiscountPercentage.Should().Be(0);

        saleItem.UpdateQuantity(4);

        saleItem.DiscountPercentage.Should().Be(10);
        saleItem.TotalAmount.Should().Be(36m);

        saleItem.UpdateQuantity(10);

        saleItem.DiscountPercentage.Should().Be(20);
        saleItem.TotalAmount.Should().Be(80m);

        saleItem.UpdateQuantity(1);

        saleItem.DiscountPercentage.Should().Be(0);
        saleItem.TotalAmount.Should().Be(10m);
    }

    [Fact(DisplayName = "UpdateUnitPrice should throw exception for negative price")]
    public void Given_SaleItem_When_UpdatingToNegativePrice_Then_ShouldThrowException()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();

        var act = () => saleItem.UpdateUnitPrice(-1m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Unit price cannot be negative");
    }

    [Fact(DisplayName = "UpdateQuantity should throw exception for negative quantity")]
    public void Given_SaleItem_When_UpdatingToNegativeQuantity_Then_ShouldThrowException()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();
        var act = () => saleItem.UpdateQuantity(-1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity cannot be negative");
    }

    [Fact(DisplayName = "UpdateQuantity should throw exception for quantity above maximum")]
    public void Given_SaleItem_When_UpdatingToQuantityAboveMax_Then_ShouldThrowException()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();
        var act = () => saleItem.UpdateQuantity(21);

        act.Should().Throw<ArgumentException>()
            .WithMessage("You cannot add more than 20 of the same item to a sale");
    }

    [Fact(DisplayName = "UpdateUnitPrice should recalculate total")]
    public void Given_SaleItem_When_UpdatingUnitPrice_Then_ShouldRecalculateTotal()
    {
        var saleItem = new SaleItem(Guid.NewGuid(), new Faker().Commerce.ProductName(), 5m, 5);
        var originalTotal = saleItem.TotalAmount;

        saleItem.UpdateUnitPrice(10m);

        saleItem.UnitPrice.Should().Be(10m);
        saleItem.DiscountAmount.Should().Be(5m);
        saleItem.TotalAmount.Should().Be(45m);
        saleItem.TotalAmount.Should().NotBe(originalTotal);
    }

    [Fact(DisplayName = "SaleItem should be created with correct values")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldSetCorrectValues()
    {
        var productId = Guid.NewGuid();
        var productName = new Faker().Commerce.ProductName();
        var unitPrice = 9.99m;
        var quantity = 2;
        var saleItem = new SaleItem(productId, productName, unitPrice, quantity);

        saleItem.ProductId.Should().Be(productId);
        saleItem.ProductName.Should().Be(productName);
        saleItem.UnitPrice.Should().Be(unitPrice);
        saleItem.Quantity.Should().Be(quantity);
        saleItem.IsCancelled.Should().BeFalse();
        saleItem.TotalAmount.Should().Be(19.98m);
        saleItem.DiscountPercentage.Should().Be(0);
        saleItem.DiscountAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Validate should return valid result for valid sale item")]
    public void Given_ValidSaleItem_When_Validated_Then_ShouldReturnValid()
    {
        var saleItem = SaleItemTestData.GenerateSaleItem();
        var result = saleItem.Validate();

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}