using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class SaleTestData
{
    private static readonly Faker<AddSaleItemCommand> AddSaleItemCommandFaker = new Faker<AddSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 999));

    private static readonly Faker<CancelSaleCommand> CancelSaleCommandFaker = new Faker<CancelSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.CancellationReason, f => f.Lorem.Sentence());

    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.BranchId, f => f.Random.Guid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.Items, f => GenerateCreateSaleItemCommands(f.Random.Int(1, 3)));

    private static readonly Faker<CreateSaleItemCommand> CreateSaleItemCommandFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 999));

    private static readonly Faker<DeleteSaleCommand> DeleteSaleCommandFaker = new Faker<DeleteSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    private static readonly Faker<GetSaleCommand> GetSaleCommandFaker = new Faker<GetSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    private static readonly Faker<UpdateSaleItemCommand> UpdateSaleItemCommandFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    public static AddSaleItemCommand GenerateAddSaleItemCommand()
    {
        return AddSaleItemCommandFaker.Generate();
    }

    public static List<AddSaleItemCommand> GenerateAddSaleItemCommands(int count)
    {
        return AddSaleItemCommandFaker.Generate(count);
    }

    public static CancelSaleCommand GenerateCancelSaleCommand()
    {
        return CancelSaleCommandFaker.Generate();
    }

    public static CreateSaleCommand GenerateCreateSaleCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }

    public static CreateSaleCommand GenerateCreateSaleCommandWithItems(int itemCount)
    {
        var command = CreateSaleCommandFaker.Generate();
        command.Items = GenerateCreateSaleItemCommands(itemCount);
        return command;
    }

    public static CreateSaleItemCommand GenerateCreateSaleItemCommand()
    {
        return CreateSaleItemCommandFaker.Generate();
    }

    public static List<CreateSaleItemCommand> GenerateCreateSaleItemCommands(int count)
    {
        return CreateSaleItemCommandFaker.Generate(count);
    }

    public static DeleteSaleCommand GenerateDeleteSaleCommand()
    {
        return DeleteSaleCommandFaker.Generate();
    }

    public static GetSaleCommand GenerateGetSaleCommand()
    {
        return GetSaleCommandFaker.Generate();
    }

    public static CancelSaleCommand GenerateInvalidCancelSaleCommand()
    {
        return new CancelSaleCommand
        {
            Id = Guid.Empty,
            CancellationReason = string.Empty
        };
    }

    public static CreateSaleCommand GenerateInvalidCreateSaleCommand()
    {
        return new CreateSaleCommand
        {
            BranchId = Guid.Empty,
            BranchName = string.Empty,
            CustomerId = Guid.Empty,
            CustomerName = string.Empty,
            Items = []
        };
    }

    public static GetSaleCommand GenerateInvalidGetSaleCommand()
    {
        return new GetSaleCommand
        {
            Id = Guid.Empty
        };
    }

    public static UpdateSaleCommand GenerateInvalidUpdateSaleCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.Empty,
            ItemsToAdd =
            [
                new() {
                    ProductId = Guid.Empty,
                    ProductName = string.Empty,
                    UnitPrice = -1,
                    Quantity = 0
                }
            ],
            ProductIdsToRemove = [Guid.Empty]
        };
    }

    public static UpdateSaleCommand GenerateUpdateSaleCommand()
    {
        var faker = new Faker();

        return new UpdateSaleCommand
        {
            Id = faker.Random.Guid(),
            ItemsToAdd = AddSaleItemCommandFaker.Generate(faker.Random.Int(0, 2)),
            ItemsToUpdate = UpdateSaleItemCommandFaker.Generate(faker.Random.Int(0, 2)),
            ProductIdsToRemove = [.. Enumerable.Range(0, faker.Random.Int(0, 2)).Select(_ => faker.Random.Guid())]
        };
    }

    public static UpdateSaleItemCommand GenerateUpdateSaleItemCommand()
    {
        return UpdateSaleItemCommandFaker.Generate();
    }

    public static List<UpdateSaleItemCommand> GenerateUpdateSaleItemCommands(int count)
    {
        return UpdateSaleItemCommandFaker.Generate(count);
    }
}