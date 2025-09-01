using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class DeleteSaleTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly DeleteSaleHandler _handler;
    private readonly ILogger<DeleteSaleHandler> _logger;

    public DeleteSaleTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
        _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _handler = new DeleteSaleHandler(_repository, _logger);
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
    public async Task DeleteSale_ShouldBeRemovedFromDatabase()
    {
        var sale = await CreateSaleInDatabase();
        var command = new DeleteSaleCommand(sale.Id);
        var result = await _handler.Handle(command, CancellationToken.None);
        var deletedSale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

        deletedSale?.Should().BeNull();
    }

    [Fact]
    public async Task DeleteSale_ShouldRemoveItemsFromDatabase()
    {
        var sale = await CreateSaleInDatabase();
        var saleItems = sale.Items.ToList();
        var command = new DeleteSaleCommand(sale.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        foreach (var item in saleItems)
        {
            var deletedItem = await _context.SaleItems.FirstOrDefaultAsync(si => si.Id == item.Id);
            deletedItem?.Should().BeNull();
        }
    }
}