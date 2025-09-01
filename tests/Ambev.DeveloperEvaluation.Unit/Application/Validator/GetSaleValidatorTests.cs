using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Validator;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleValidator"/> class.
/// Tests cover validation scenarios for GetSaleCommand.
/// </summary>
public class GetSaleValidatorTests
{
    private readonly GetSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleValidatorTests"/> class.
    /// </summary>
    public GetSaleValidatorTests()
    {
        _validator = new GetSaleValidator();
    }

    /// <summary>
    /// Tests that a command created with constructor passes validation.
    /// </summary>
    [Fact(DisplayName = "Given command created with constructor When validating Then should pass")]
    public void Validate_CommandWithConstructor_ShouldPass()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that a command with empty ID fails validation.
    /// </summary>
    [Fact(DisplayName = "Given command with empty ID When validating Then should fail")]
    public void Validate_EmptyId_ShouldFail()
    {
        // Given
        var command = SaleTestData.GenerateGetSaleCommand();

        command.Id = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Sale ID cannot be empty");
    }

    /// <summary>
    /// Tests that the command validate method works correctly for invalid data.
    /// </summary>
    [Fact(DisplayName = "Given invalid command When calling Validate method Then should return invalid result")]
    public void Validate_InvalidCommandWithMethod_ShouldReturnInvalidResult()
    {
        // Given
        var command = SaleTestData.GenerateInvalidGetSaleCommand();

        // When
        var result = command.Validate();

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that a valid get sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid get sale command When validating Then should pass")]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Given
        var command = SaleTestData.GenerateGetSaleCommand();

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the command validate method works correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid command When calling Validate method Then should return valid result")]
    public void Validate_ValidCommandWithMethod_ShouldReturnValidResult()
    {
        // Given
        var command = SaleTestData.GenerateGetSaleCommand();

        // When
        var result = command.Validate();

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}