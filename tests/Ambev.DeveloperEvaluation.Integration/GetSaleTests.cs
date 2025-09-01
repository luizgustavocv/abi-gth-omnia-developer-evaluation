using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class GetSaleTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly GetSaleHandler _handler;
    private readonly IMapper _mapper;

    public GetSaleTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_repository, _mapper);

        SetupMapper();
    }

    private async Task<Sale> CreateCancelledSaleInDatabase()
    {
        var sale = await CreateSaleInDatabase();

        sale.Cancel();

        await _context.SaveChangesAsync();
        return sale;
    }

    private async Task<Sale> CreateSaleInDatabase()
    {
        var faker = new Faker();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            SaleDate = DateTime.UtcNow,
            SaleNumber = faker.Random.Long(0, 9999999999),
            Status = SaleStatus.Confirmed
        };

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            DiscountPercentage = 0,
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Int(1, 3),
            SaleId = sale.Id,
            UnitPrice = faker.Random.Decimal(1, 999)
        };

        sale.Items.Add(item);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return sale;
    }

    private async Task<Sale> CreateSaleWithMultipleItemsInDatabase()
    {
        var faker = new Faker();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            SaleDate = DateTime.UtcNow,
            SaleNumber = faker.Random.Long(0, 9999999999),
            Status = SaleStatus.Confirmed
        };

        var item1 = new SaleItem
        {
            Id = Guid.NewGuid(),
            DiscountPercentage = 10m,
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = 4,
            SaleId = sale.Id,
            UnitPrice = faker.Random.Decimal(1, 999)
        };

        var item2 = new SaleItem
        {
            Id = Guid.NewGuid(),
            DiscountPercentage = 20m,
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = 10,
            SaleId = sale.Id,
            UnitPrice = faker.Random.Decimal(1, 999)
        };

        sale.Items.Add(item1);
        sale.Items.Add(item2);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return sale;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Execute_CancelledSale_ShouldReturnCancelledStatus()
    {
        var sale = await CreateCancelledSaleInDatabase();
        var command = new GetSaleCommand { Id = sale.Id };
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(SaleStatus.Cancelled);
        result.Items.Should().AllSatisfy(item => item.IsCancelled.Should().BeTrue());
    }

    [Fact]
    public async Task Execute_ExistingSale_ShouldReturnSaleFromDatabase()
    {
        var sale = await CreateSaleInDatabase();
        var command = new GetSaleCommand { Id = sale.Id };
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.CustomerId.Should().Be(sale.CustomerId);
        result.BranchId.Should().Be(sale.BranchId);
        result.Items.Should().HaveCount(sale.Items.Count);
    }

    [Fact]
    public async Task Execute_NonExistentSale_ShouldThrowArgumentException()
    {
        var command = new GetSaleCommand { Id = Guid.NewGuid() };
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*not found");
    }

    [Fact]
    public async Task Execute_SaleWithItems_ShouldIncludeAllItemDetails()
    {
        var sale = await CreateSaleWithMultipleItemsInDatabase();
        var command = new GetSaleCommand { Id = sale.Id };
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(item =>
        {
            item.ProductId.Should().NotBeEmpty();
            item.ProductName.Should().NotBeNullOrEmpty();
            item.UnitPrice.Should().BeGreaterThan(0);
            item.Quantity.Should().BeGreaterThan(0);
        });
    }

    private void SetupMapper()
    {
        _mapper.Map<GetSaleResult>(Arg.Any<Sale>()).Returns(callInfo =>
        {
            var sale = callInfo.Arg<Sale>();

            return new GetSaleResult
            {
                Id = sale.Id,
                BranchId = sale.BranchId,
                CustomerId = sale.CustomerId,
                SaleDate = sale.SaleDate,
                SaleNumber = sale.SaleNumber,
                Status = sale.Status,
                TotalAmount = sale.TotalAmount,
                Items = sale.Items.Select(i =>
                new GetSaleItemResult
                {
                    Id = i.Id,
                    DiscountAmount = i.DiscountAmount,
                    DiscountPercentage = i.DiscountPercentage,
                    IsCancelled = i.IsCancelled,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    TotalAmount = i.TotalAmount,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        });
    }
}