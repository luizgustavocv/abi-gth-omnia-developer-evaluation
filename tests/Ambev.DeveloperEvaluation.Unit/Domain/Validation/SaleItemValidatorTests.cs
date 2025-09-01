using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the <see cref="SaleItemValidator"/> class.
/// Tests cover validation scenarios for SaleItem entities.
/// </summary>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    [Fact(DisplayName = "Given SaleItemValidator When created Then should not be null")]
    public void SaleItemValidator_WhenCreated_ShouldNotBeNull()
    {
        // Given & When
        var validator = new SaleItemValidator();

        // Then
        validator.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given cancelled sale item When validating Then should pass")]
    public void Validate_CancelledSaleItem_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateCancelledSaleItem();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale item with 10% discount When validating Then should pass")]
    public void Validate_SaleItemWith10PercentDiscount_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateSaleItemWith10PercentDiscount();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale item with 20% discount When validating Then should pass")]
    public void Validate_SaleItemWith20PercentDiscount_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateSaleItemWith20PercentDiscount();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale item with maximum quantity When validating Then should pass")]
    public void Validate_SaleItemWithMaxQuantity_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateSaleItemWithMaxQuantity();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale item with no discount When validating Then should pass")]
    public void Validate_SaleItemWithNoDiscount_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateSaleItem();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given valid sale item When validating Then should pass")]
    public void Validate_ValidSaleItem_ShouldPass()
    {
        // Given
        var saleItem = SaleItemTestData.GenerateSaleItem();

        // When
        var result = _validator.Validate(saleItem);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}