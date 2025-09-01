using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    public static SaleItem GenerateCancelledSaleItem()
    {
        var item = GenerateSaleItem();

        item.Cancel();

        return item;
    }

    public static SaleItem GenerateSaleItem()
    {
        var faker = new Faker();

        return new SaleItem(
            faker.Random.Guid(),
            faker.Commerce.ProductName(),
            faker.Random.Decimal(1, 999),
            faker.Random.Int(1, 3)
        );
    }

    public static SaleItem GenerateSaleItemWith10PercentDiscount()
    {
        var item = GenerateSaleItem();

        item.UpdateQuantity(new Faker().Random.Int(4, 9));

        return item;
    }

    public static SaleItem GenerateSaleItemWith20PercentDiscount()
    {
        var item = GenerateSaleItem();

        item.UpdateQuantity(new Faker().Random.Int(10, 20));

        return item;
    }

    public static SaleItem GenerateSaleItemWithMaxQuantity()
    {
        var item = GenerateSaleItem();

        item.UpdateQuantity(20);

        return item;
    }
}