using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handler;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _logger);
    }

    /// <summary>
    /// Tests that adding items to sale works correctly.
    /// </summary>
    [Fact(DisplayName = "Given items to add When updating sale Then adds items successfully")]
    public async Task Handle_AddItems_AddsItemsSuccessfully()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd = TestData.SaleTestData.GenerateAddSaleItemCommands(2);
        command.ItemsToUpdate.Clear();
        command.ProductIdsToRemove.Clear();

        var sale = SaleTestData.GenerateSale();
        var updatedSale = SaleTestData.GenerateSaleWithItems(2);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Message.Should().Contain("added");

        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating a cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When updating sale Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();
        var cancelledSale = SaleTestData.GenerateCancelledSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Canceled sales cannot be updated");
    }

    /// <summary>
    /// Tests removing items with empty GUID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty GUID to remove When updating sale Then throws validation exception")]
    public async Task Handle_EmptyGuidToRemove_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ProductIdsToRemove = new List<Guid> { Guid.Empty };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that validation is performed before repository calls.
    /// </summary>
    [Fact(DisplayName = "Given invalid command When handling Then validates before repository calls")]
    public async Task Handle_InvalidCommand_ValidatesBeforeRepositoryCalls()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests adding items with invalid data throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid items to add When updating sale Then throws validation exception")]
    public async Task Handle_InvalidItemsToAdd_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd = new List<AddSaleItemCommand>
        {
            new AddSaleItemCommand
            {
                ProductId = Guid.Empty,
                ProductName = string.Empty,
                UnitPrice = -1,
                Quantity = 0
            }
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests updating items with invalid data throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid items to update When updating sale Then throws validation exception")]
    public async Task Handle_InvalidItemsToUpdate_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToUpdate = new List<UpdateSaleItemCommand>
        {
            new UpdateSaleItemCommand
            {
                ProductId = Guid.Empty,
                Quantity = 0
            }
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that an invalid update sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid update data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateInvalidUpdateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that multiple operations can be performed in a single update.
    /// </summary>
    [Fact(DisplayName = "Given multiple operations When updating sale Then performs all operations")]
    public async Task Handle_MultipleOperations_PerformsAllOperations()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd = TestData.SaleTestData.GenerateAddSaleItemCommands(1);
        command.ItemsToUpdate = TestData.SaleTestData.GenerateUpdateSaleItemCommands(1);

        var sale = SaleTestData.GenerateSaleWithItems(2);
        var existingItem = sale.Items.First();

        command.ItemsToUpdate.First().ProductId = existingItem.ProductId;
        command.ProductIdsToRemove = new List<Guid> { sale.Items.Last().ProductId };

        var updatedSale = SaleTestData.GenerateSaleWithItems(2);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Message.Should().Contain("added");
        result.Message.Should().Contain("updated");
        result.Message.Should().Contain("removed");

        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating a non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When updating sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that removing items from sale works correctly.
    /// </summary>
    [Fact(DisplayName = "Given items to remove When updating sale Then removes items successfully")]
    public async Task Handle_RemoveItems_RemovesItemsSuccessfully()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd.Clear();
        command.ItemsToUpdate.Clear();

        var sale = SaleTestData.GenerateSaleWithItems(2);
        var existingItem = sale.Items.First();

        command.ProductIdsToRemove = new List<Guid> { existingItem.ProductId };

        var updatedSale = SaleTestData.GenerateSaleWithItems(1);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Message.Should().Contain("removed");

        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that update result contains correct information.
    /// </summary>
    [Fact(DisplayName = "Given successful update When handling Then returns correct result information")]
    public async Task Handle_SuccessfulUpdate_ReturnsCorrectResultInformation()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd = TestData.SaleTestData.GenerateAddSaleItemCommands(1);

        var sale = SaleTestData.GenerateSale();
        var updatedSale = SaleTestData.GenerateSaleWithItems(1);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(updatedSale.Id);
        result.SaleNumber.Should().Be(updatedSale.SaleNumber);
        result.TotalAmount.Should().Be(updatedSale.TotalAmount);
        result.Status.Should().Be(updatedSale.Status);
        result.ItemCount.Should().Be(updatedSale.Items.Count);
        result.UpdatedAt.Should().BeAfter(DateTime.MinValue);
        result.Message.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that updating item quantities works correctly.
    /// </summary>
    [Fact(DisplayName = "Given items to update When updating sale Then updates quantities successfully")]
    public async Task Handle_UpdateItems_UpdatesQuantitiesSuccessfully()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();

        command.ItemsToAdd.Clear();
        command.ItemsToUpdate = TestData.SaleTestData.GenerateUpdateSaleItemCommands(1);
        command.ProductIdsToRemove.Clear();

        var sale = SaleTestData.GenerateSaleWithItems(1);
        var existingItem = sale.Items.First();

        command.ItemsToUpdate.First().ProductId = existingItem.ProductId;

        var updatedSale = SaleTestData.GenerateSaleWithItems(1);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Message.Should().Contain("updated");

        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository methods correctly")]
    public async Task Handle_ValidRequest_CallsRepositoryCorrectly()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();
        var sale = SaleTestData.GenerateSale();
        var updatedSale = SaleTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a valid update sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid update request When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = TestData.SaleTestData.GenerateUpdateSaleCommand();
        var sale = SaleTestData.GenerateSaleWithItems(1);
        var updatedSale = SaleTestData.GenerateSaleWithItems(2);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(updatedSale.Id);
        result.SaleNumber.Should().Be(updatedSale.SaleNumber);
        result.TotalAmount.Should().Be(updatedSale.TotalAmount);
        result.Status.Should().Be(updatedSale.Status);

        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}