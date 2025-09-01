using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class UpdateSaleTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly UpdateSaleHandler _handler;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_repository, _logger);
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
            SaleId = sale.Id,
            Quantity = faker.Random.Int(1, 3),
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

        for (int i = 0; i < 3; i++)
        {
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
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return sale;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Execute_AddNewItem_ShouldBePersistedItemInDatabase()
    {
        var sale = await CreateSaleInDatabase();
        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            ItemsToAdd = new List<AddSaleItemCommand>
            {
                GenerateAddSaleItemCommand()
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);
        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        
        updatedSale?.Items.Should().HaveCount(2);
        
        var newItem = updatedSale?.Items.FirstOrDefault(i => i.ProductId == command.ItemsToAdd.First().ProductId);
        
        newItem?.Should().NotBeNull();
        newItem?.Quantity.Should().Be(command.ItemsToAdd.First().Quantity);
    }

    [Fact]
    public async Task Execute_RemoveItem_ShouldBeDeletedItemInDatabase()
    {
        var sale = await CreateSaleWithMultipleItemsInDatabase();
        var itemToRemove = sale.Items.First();

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            ProductIdsToRemove = new List<Guid> { itemToRemove.ProductId }
        };

        var result = await _handler.Handle(command, CancellationToken.None);
        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);

        updatedSale?.Items.Should().HaveCount(2);
        updatedSale?.Items.Should().NotContain(i => i.ProductId == itemToRemove.ProductId);
    }

    [Fact]
    public async Task Execute_UpdateCancelledSale_ShouldThrowException()
    {
        var sale = await CreateCancelledSaleInDatabase();
        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            ItemsToAdd = new List<AddSaleItemCommand> { GenerateAddSaleItemCommand() }
        };

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Canceled sales cannot be updated");
    }

    [Fact]
    public async Task Execute_UpdateExistingItem_ShouldModifyItemInDatabase()
    {
        var sale = await CreateSaleInDatabase();
        var existingItem = sale.Items.First();
        var newQuantity = existingItem.Quantity + 10;
        
        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            ItemsToUpdate = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = existingItem.ProductId,
                    Quantity = newQuantity
                }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);
        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        var updatedItem = updatedSale?.Items.First(i => i.ProductId == existingItem.ProductId);
        
        updatedItem?.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public async Task Execute_UpdateWithDiscounts_ShouldCalculateDiscounts()
    {
        var faker = new Faker();
        var sale = await CreateSaleInDatabase();

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            ItemsToAdd = new List<AddSaleItemCommand>
            {
                new AddSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 4,
                    UnitPrice = 10m
                }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);
        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        var discountedItem = updatedSale?.Items.First(i => i.Quantity == 4);
        
        discountedItem?.DiscountPercentage.Should().Be(10m);
    }

    private AddSaleItemCommand GenerateAddSaleItemCommand()
    {
        var faker = new Faker();

        return new AddSaleItemCommand
        {
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Int(1, 3),
            UnitPrice = faker.Random.Decimal(1, 999)
        };
    }
}