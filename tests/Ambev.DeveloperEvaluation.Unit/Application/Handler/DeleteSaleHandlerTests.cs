using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handler;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _handler = new DeleteSaleHandler(_saleRepository, _logger);
    }

    /// <summary>
    /// Tests that command validation with empty ID fails.
    /// </summary>
    [Fact(DisplayName = "Given command with no ID When validating Then should fail")]
    public async Task Handle_CommandWithNoID_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateDeleteSaleCommand();
        command.Id = Guid.Empty;

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that delete sale request returns success response.
    /// </summary>
    [Fact(DisplayName = "Given sale data When deleting sale Then returns success response")]
    public async Task Handle_DeleteRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = TestData.SaleTestData.GenerateDeleteSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();

        sale.Id = command.Id;

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.DeleteAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var deleteSaleResult = await _handler.Handle(command, CancellationToken.None);

        deleteSaleResult.Should().NotBeNull();
        deleteSaleResult.Success.Should().BeTrue();

        // Then
        await _saleRepository.Received(1).DeleteAsync(sale.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that deleting a non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When deleting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateDeleteSaleCommand();

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }
}