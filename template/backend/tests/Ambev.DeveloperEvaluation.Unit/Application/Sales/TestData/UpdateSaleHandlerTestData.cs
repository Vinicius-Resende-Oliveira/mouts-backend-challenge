using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// </summary>
    private static readonly Faker<UpdateSaleCommand> createSaleCommandHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(u => u.Id, Guid.NewGuid())
        .RuleFor(u => u.SaleDate, f => DateTime.UtcNow)
        .RuleFor(u => u.Customer, f => f.Internet.UserName())
        .RuleFor(u => u.Branch, f => f.Commerce.Department())
        .RuleFor(u => u.Items, f => GenerateListUpdateSaleItemComand(f.Random.Int(0, 99)));

    private static readonly Faker<UpdateSaleItemCommand> createSaleItemCommandHandlerFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(u => u.ProductId, Guid.NewGuid())
        .RuleFor(u => u.Quantity, f => f.Random.Int(1,20));

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return createSaleCommandHandlerFaker.Generate();
    }

    public static UpdateSaleItemCommand GenerateValidSaleItemCommand()
    {
        return createSaleItemCommandHandlerFaker.Generate();
    }

    public static UpdateSaleCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<UpdateSaleCommand>();
        command.Customer = String.Empty;
        command.Branch = String.Empty;
        command.Items = GenerateListUpdateSaleItemComand(0);
        return command;
    }

    public static List<UpdateSaleItemCommand> GenerateListUpdateSaleItemComand(int count = 0)
    {
        var list = new List<UpdateSaleItemCommand>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidSaleItemCommand());
        }
        return list;
    }

    public static UpdateSaleResult GenerateValidResult(Sale sale)
    {
        return new UpdateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Customer = sale.Customer,
            Branch = sale.Branch,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items?.Select(i => new GetSaleItemResult
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalValue = i.TotalValue,
                Discount = i.Discount
            }).ToList() ?? new List<GetSaleItemResult>()
        };
    }
}
