using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.CreatedAt, f => f.Date.Recent(5))
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.SaleDate, f => f.Date.Recent(5))
        .RuleFor(s => s.Status, f => SaleStatus.Confirmed)
        .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(1));

    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateSale();

        sale.Cancel();

        return sale;
    }

    public static Sale GenerateSale()
    {
        return SaleFaker.Generate();
    }

    public static Sale GenerateSaleEmpty()
    {
        var sale = GenerateSale();

        sale.Items.Clear();

        return sale;
    }

    public static Sale GenerateSaleWithItems(int itemCount = 3)
    {
        var sale = GenerateSale();
        
        for (int i = 0; i < itemCount; i++)
        {
            var item = SaleItemTestData.GenerateSaleItem();
            sale.AddItem(item);
        }

        return sale;
    }

    public static Sale GenerateSaleWithMaxItemsQuantity()
    {
        var sale = GenerateSale();
        var item = SaleItemTestData.GenerateSaleItem();
        
        item.UpdateQuantity(20);
        sale.AddItem(item);
        
        return sale;
    }
}