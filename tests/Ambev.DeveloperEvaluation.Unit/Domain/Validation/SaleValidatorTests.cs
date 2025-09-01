using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the <see cref="SaleValidator"/> class.
/// Tests cover validation scenarios for Sale entities.
/// </summary>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
    }

    [Fact(DisplayName = "Given SaleValidator When created Then should not be null")]
    public void SaleValidator_WhenCreated_ShouldNotBeNull()
    {
        // Given & When
        var validator = new SaleValidator();

        // Then
        validator.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given cancelled sale When validating Then should pass")]
    public void Validate_CancelledSale_ShouldPass()
    {
        // Given
        var sale = SaleTestData.GenerateCancelledSale();

        // When
        var result = _validator.Validate(sale);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given empty sale When validating Then should pass basic validation")]
    public void Validate_EmptySale_ShouldPassBasicValidation()
    {
        // Given
        var sale = SaleTestData.GenerateSaleEmpty();

        // When
        var result = _validator.Validate(sale);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale with maximum items When validating Then should pass")]
    public void Validate_SaleWithMaxItems_ShouldPass()
    {
        // Given
        var sale = SaleTestData.GenerateSaleWithMaxItemsQuantity();

        // When
        var result = _validator.Validate(sale);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale with valid items When validating Then should pass")]
    public void Validate_SaleWithValidItems_ShouldPass()
    {
        // Given
        var sale = SaleTestData.GenerateSaleWithItems(3);

        // When
        var result = _validator.Validate(sale);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given valid sale When validating Then should pass")]
    public void Validate_ValidSale_ShouldPass()
    {
        // Given
        var sale = SaleTestData.GenerateSale();

        // When
        var result = _validator.Validate(sale);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}