using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleItemHandlerTestData
{
    private static readonly Faker<SaleItem> createSaleItemCommandHandlerFaker = new Faker<SaleItem>()
        .RuleFor(u => u.ProductId, f => Guid.NewGuid())
        .RuleFor(u => u.UnitPrice, f => f.Random.Decimal(1, 999))
        .RuleFor(u => u.TotalValue, f => f.Random.Decimal(1, 9999))
        .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.Quantity, f => f.Random.Int(1,  21));

    public static SaleItem GenerateValidSaleItem()
        => createSaleItemCommandHandlerFaker.Generate();

    public static SaleItem GenerateInvalidSaleItem()
        => Substitute.ForPartsOf<SaleItem>(Guid.Empty, 0, 0);

    public static List<SaleItem> GenerateListSaleItem(int count = 0)
    {
        var list = new List<SaleItem>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidSaleItem());
        }
        return list;
    }
}
