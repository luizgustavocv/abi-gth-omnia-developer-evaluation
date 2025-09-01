using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class CreateSaleTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly CreateSaleHandler _handler;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_repository, _mapper, _logger);

        SetupMapper();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Execute_MultipleSales_ShouldGenerateUniqueSaleNumbers()
    {
        var command1 = GenerateCreateSaleCommand();
        var command2 = GenerateCreateSaleCommand();
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);
        var sale1 = await _context.Sales.FindAsync(result1.Id);
        var sale2 = await _context.Sales.FindAsync(result2.Id);

        sale1?.SaleNumber.Should().NotBe(sale2?.SaleNumber);
    }

    [Fact]
    public async Task Execute_SaleWithBusinessRuleViolation_ShouldThrowException()
    {
        var command = GenerateCreateSaleCommandWithTooManyItems();
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("*You cannot add more than 20 of the same item to a sale*");
    }

    [Fact]
    public async Task Execute_SaleWithMultipleItems_ShouldDetermineDiscountsAccurately()
    {
        var command = GenerateCreateSaleCommandWithDiscount();
        var result = await _handler.Handle(command, CancellationToken.None);
        var savedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == result.Id);
        var itemWith10Discount = savedSale?.Items.First(i => i.Quantity >= 4 && i.Quantity < 10);
        var itemWith20Discount = savedSale?.Items.First(i => i.Quantity >= 10);

        itemWith10Discount?.DiscountPercentage.Should().Be(10m);
        itemWith20Discount?.DiscountPercentage.Should().Be(20m);
    }

    [Fact]
    public async Task Execute_ValidSale_ShouldStoreInDatabas()
    {
        var command = GenerateCreateSaleCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        var savedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == result.Id);

        savedSale.Should().NotBeNull();
        savedSale?.CustomerId.Should().Be(command.CustomerId);
        savedSale?.BranchId.Should().Be(command.BranchId);
        savedSale?.Items.Should().HaveCount(command.Items.Count);
    }

    private static CreateSaleCommand GenerateCreateSaleCommand()
    {
        var faker = new Faker();

        return new CreateSaleCommand
        {
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = faker.Random.Guid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = faker.Random.Int(1, 3),
                    UnitPrice = faker.Random.Decimal(1, 999)
                }
            }
        };
    }

    private static CreateSaleCommand GenerateCreateSaleCommandWithDiscount()
    {
        var faker = new Faker();

        return new CreateSaleCommand
        {
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = faker.Random.Guid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 4,
                    UnitPrice = faker.Random.Decimal(1, 999)
                },
                new CreateSaleItemCommand
                {
                    ProductId = faker.Random.Guid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 10,
                    UnitPrice = faker.Random.Decimal(1, 999)
                }
            }
        };
    }

    private static CreateSaleCommand GenerateCreateSaleCommandWithTooManyItems()
    {
        var faker = new Faker();

        return new CreateSaleCommand
        {
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = faker.Random.Guid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 21,
                    UnitPrice = faker.Random.Decimal(1, 999)
                }
            }
        };
    }

    private void SetupMapper()
    {
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(callInfo =>
        {
            var sale = callInfo.Arg<Sale>();

            return new CreateSaleResult
            {
                Id = sale.Id,
                BranchId = sale.BranchId,
                CustomerId = sale.CustomerId,
                SaleDate = sale.SaleDate,
                SaleNumber = sale.SaleNumber,
                Status = sale.Status,
                TotalAmount = sale.TotalAmount
            };
        });
    }
}