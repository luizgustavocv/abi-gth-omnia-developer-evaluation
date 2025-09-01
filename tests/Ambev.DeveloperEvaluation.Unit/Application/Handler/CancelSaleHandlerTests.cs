using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handler;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _logger);
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given already cancelled sale When cancelling sale Then throws InvalidOperationException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        var cancelledSale = Domain.Entities.TestData.SaleTestData.GenerateCancelledSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale has already been cancelled");
    }

    /// <summary>
    /// Tests that command constructor works correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale ID and reason When creating command Then sets properties correctly")]
    public async Task Handle_CommandWithConstructor_SetsPropertiesCorrectly()
    {
        // Given
        var saleId = Guid.NewGuid();
        var reason = "Test cancellation reason";
        var command = new CancelSaleCommand(saleId, reason);
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        command.Id.Should().Be(saleId);
        command.CancellationReason.Should().Be(reason);
        result.SaleId.Should().Be(sale.Id);

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that default cancellation reason is used when not specified.
    /// </summary>
    [Fact(DisplayName = "Given command without reason When creating Then uses default reason")]
    public void Handle_CommandWithoutReason_UsesDefaultReason()
    {
        // Given & When
        var command = new CancelSaleCommand();

        // Then
        command.CancellationReason.Should().Be("User cancelled the sale");
    }

    /// <summary>
    /// Tests that validation is performed before repository calls.
    /// </summary>
    [Fact(DisplayName = "Given invalid command When handling Then validates before repository calls")]
    public async Task Handle_InvalidCommand_ValidatesBeforeRepositoryCalls()
    {
        // Given
        var command = new CancelSaleCommand
        {
            Id = Guid.Empty,
            CancellationReason = ""
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that command validation works correctly.
    /// </summary>
    [Theory(DisplayName = "Given different invalid inputs When validating Then should fail appropriately")]
    [InlineData("00000000-0000-0000-0000-000000000000", "reason")]
    [InlineData("a73196b7-33a0-4d66-850f-64de6d9bf679", "")]
    [InlineData("00000000-0000-0000-0000-000000000000", "")]
    public async Task Handle_InvalidInputs_ThrowsValidationException(string guidString, string reason)
    {
        // Given
        var command = new CancelSaleCommand
        {
            Id = Guid.Parse(guidString),
            CancellationReason = reason
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that an invalid cancel sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid cancel data When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateInvalidCancelSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that cancelling a non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When cancelling sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that the result contains accurate cancellation timestamp.
    /// </summary>
    [Fact(DisplayName = "Given successful cancellation When handling Then result contains accurate timestamp")]
    public async Task Handle_SuccessfulCancellation_ResultContainsAccurateTimestamp()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var beforeCancellation = DateTime.UtcNow;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterCancellation = DateTime.UtcNow;

        // Then
        result.CancelledAt.Should().BeAfter(beforeCancellation);
        result.CancelledAt.Should().BeBefore(afterCancellation);
    }

    /// <summary>
    /// Tests that repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository methods correctly")]
    public async Task Handle_ValidRequest_CallsRepositoryCorrectly()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a valid cancel sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid cancel request When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSaleWithItems(2);
        
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.SaleId.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.Message.Should().Contain("Sale has been cancelled successfully");
        result.CancelledAt.Should().BeAfter(DateTime.MinValue);

        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the sale status is updated to cancelled.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When cancelling Then updates sale status to cancelled")]
    public async Task Handle_ValidSale_UpdatesSaleStatusToCancelled()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSaleWithItems(1);
        sale.Status = SaleStatus.Confirmed;
        
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.UpdatedAt.Should().NotBeNull();

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Status == SaleStatus.Cancelled), 
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that cancellation reason length validation works.
    /// </summary>
    [Fact(DisplayName = "Given very long cancellation reason When validating Then should fail")]
    public async Task Handle_VeryLongCancellationReason_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCancelSaleCommand();
        command.CancellationReason = new string('x', 501);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}