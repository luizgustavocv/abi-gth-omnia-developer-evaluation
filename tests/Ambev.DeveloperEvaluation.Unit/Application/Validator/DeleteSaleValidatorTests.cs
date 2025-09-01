using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Validator;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleValidator"/> class.
/// Tests cover validation scenarios for DeleteSaleCommand.
/// </summary>
public class DeleteSaleValidatorTests
{
    private readonly DeleteSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleValidatorTests"/> class.
    /// </summary>
    public DeleteSaleValidatorTests()
    {
        _validator = new DeleteSaleValidator();
    }

    /// <summary>
    /// Tests that a command deleted with constructor passes validation.
    /// </summary>
    [Fact(DisplayName = "Given command deleted with constructor When validating Then should pass")]
    public void Validate_CommandWithConstructor_ShouldPass()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

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
    public void Validate_EmptyID_ShouldFail()
    {
        // Given
        var command = SaleTestData.GenerateDeleteSaleCommand();

        command.Id = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Sale ID is required");
    }
}