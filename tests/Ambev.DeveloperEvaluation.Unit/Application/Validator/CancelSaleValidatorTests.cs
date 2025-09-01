using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Validator;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleValidator"/> class.
/// Tests cover validation scenarios for CancelSaleCommand.
/// </summary>
public class CancelSaleValidatorTests
{
    private readonly CancelSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleValidatorTests"/> class.
    /// </summary>
    public CancelSaleValidatorTests()
    {
        _validator = new CancelSaleValidator();
    }

    /// <summary>
    /// Tests edge cases for cancellation reason length.
    /// </summary>
    [Theory(DisplayName = "Given cancellation reason with different lengths When validating Then should validate correctly")]
    [InlineData(1, true)]
    [InlineData(500, true)]
    [InlineData(501, false)]
    public void Validate_CancellationReasonLength_ShouldValidateCorrectly(int length, bool shouldBeValid)
    {
        // Given
        var command = SaleTestData.GenerateCancelSaleCommand();

        command.CancellationReason = new string('x', length);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().Be(shouldBeValid);
    }

    /// <summary>
    /// Tests that a command created with constructor passes validation.
    /// </summary>
    [Fact(DisplayName = "Given command created with constructor When validating Then should pass")]
    public void Validate_CommandWithConstructor_ShouldPass()
    {
        // Given
        var saleId = Guid.NewGuid();
        var reason = "reason";
        var command = new CancelSaleCommand(saleId, reason);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that a command created with default constructor has default values.
    /// </summary>
    [Fact(DisplayName = "Given command created with default constructor When validating Then should have default reason")]
    public void Validate_CommandWithDefaultConstructor_ShouldHaveDefaultReason()
    {
        // Given
        var command = new CancelSaleCommand
        {
            Id = Guid.NewGuid()
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        command.CancellationReason.Should().Be("User cancelled the sale");
    }

    /// <summary>
    /// Tests that a command with empty cancellation reason fails validation.
    /// </summary>
    [Fact(DisplayName = "Given command with empty cancellation reason When validating Then should fail")]
    public void Validate_EmptyCancellationReason_ShouldFail()
    {
        // Given
        var command = SaleTestData.GenerateCancelSaleCommand();

        command.CancellationReason = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Cancellation reason is required");
    }

    /// <summary>
    /// Tests that a command with empty ID fails validation.
    /// </summary>
    [Fact(DisplayName = "Given command with empty ID When validating Then should fail")]
    public void Validate_EmptyId_ShouldFail()
    {
        // Given
        var command = SaleTestData.GenerateCancelSaleCommand();

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
        var command = SaleTestData.GenerateInvalidCancelSaleCommand();

        // When
        var result = command.Validate();

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that a command with cancellation reason exceeding max length fails validation.
    /// </summary>
    [Fact(DisplayName = "Given command with long cancellation reason When validating Then should fail")]
    public void Validate_LongCancellationReason_ShouldFail()
    {
        // Given
        var command = SaleTestData.GenerateCancelSaleCommand();

        command.CancellationReason = new string('x', 501);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Cancellation reason must contain between 1 and 500 characters");
    }

    /// <summary>
    /// Tests that a valid cancel sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid cancel sale command When validating Then should pass")]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Given
        var command = SaleTestData.GenerateCancelSaleCommand();

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
        var command = SaleTestData.GenerateCancelSaleCommand();

        // When
        var result = command.Validate();

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}