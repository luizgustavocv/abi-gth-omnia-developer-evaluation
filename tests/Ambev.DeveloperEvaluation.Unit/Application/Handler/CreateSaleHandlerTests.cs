using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ValidationException = FluentValidation.ValidationException;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handler;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    /// <summary>
    /// Tests that command validation with empty items list fails.
    /// </summary>
    [Fact(DisplayName = "Given command with no items When validating Then should fail")]
    public async Task Handle_CommandWithNoItems_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();
        command.Items.Clear();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that an invalid sale creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = TestData.SaleTestData.GenerateInvalidCreateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that sale items are created with correct business rules applied.
    /// </summary>
    [Theory(DisplayName = "Given items with different quantities When creating sale Then applies correct discounts")]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(10)]
    public async Task Handle_ItemsWithDifferentQuantities_AppliesCorrectDiscounts(int quantity)
    {
        // Given
        var faker = new Faker();
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();

        command.Items.Clear();

        command.Items.Add(new CreateSaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = 10m
        });

        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.Items.Any()),
            Arg.Any<CancellationToken>());

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that sale validation is performed and errors are handled.
    /// </summary>
    [Fact(DisplayName = "Given sale with validation errors When creating sale Then throws validation exception")]
    public void Handle_SaleValidationFails_ThrowsValidationException()
    {
        // Given
        var faker = new Faker();
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();

        var invalidSale = new Sale
        {
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            CustomerId = Guid.Empty,
            CustomerName = command.CustomerName
        };

        // When & Then
        var act = () =>
        {
            var sale = new Sale
            {
                BranchId = command.BranchId,
                BranchName = command.BranchName,
                CustomerId = command.CustomerId,
                CustomerName = command.CustomerName
            };

            var maxItem = new SaleItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                10m,
                20
            );

            sale.AddItem(maxItem);

            var extraItem = new SaleItem(
                maxItem.ProductId,
                faker.Commerce.ProductName(),
                10m,
                1
            );

            sale.AddItem(extraItem);
        };

        act.Should().Throw<ArgumentException>()
           .WithMessage("You cannot add more than 20 of the same item to a sale");
    }

    /// <summary>
    /// Tests that repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When handling Then calls repository with correct sale")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectSale()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.BranchId == command.BranchId &&
                s.BranchName == command.BranchName &&
                s.CustomerId == command.CustomerId &&
                s.CustomerName == command.CustomerName &&
                s.Items.Count == command.Items.Count),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When handling Then maps sale to result correctly")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that a valid sale creation request returns success response.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCreateSaleCommand();
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        createSaleResult.SaleNumber.Should().Be(sale.SaleNumber);

        // Then
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that sale items are properly created and added to the sale.
    /// </summary>
    [Fact(DisplayName = "Given sale command with items When creating sale Then creates sale with items")]
    public async Task Handle_ValidRequestWithItems_CreatesSaleWithItems()
    {
        // Given
        var command = TestData.SaleTestData.GenerateCreateSaleCommandWithItems(2);
        var sale = Domain.Entities.TestData.SaleTestData.GenerateSale();
        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.BranchId == command.BranchId &&
                              s.BranchName == command.BranchName &&
                              s.CustomerId == command.CustomerId &&
                              s.CustomerName == command.CustomerName),
            Arg.Any<CancellationToken>());
    }
}