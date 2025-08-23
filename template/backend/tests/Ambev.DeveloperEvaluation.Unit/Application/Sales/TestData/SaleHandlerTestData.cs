using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class SaleHandlerTestData
{
    private static readonly Faker<Sale> createSaleHandlerFaker = new Faker<Sale>()
        .RuleFor(u => u.Id, f => Guid.NewGuid())
        .RuleFor(u => u.SaleNumber, f => f.Random.Int(1, 999))
        .RuleFor(u => u.Customer, f => f.Internet.UserName())
        .RuleFor(u => u.Branch, f => f.Commerce.Department())
        .RuleFor(u => u.IsCancelled, f => false)
        .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.Items, f => SaleItemHandlerTestData.GenerateListSaleItem(f.Random.Int(1, 9999)) );

    public static Sale GenerateValidSale()
    {
        return createSaleHandlerFaker.Generate();
    }

    public static Sale GenerateInvalidSale()
        => Substitute.ForPartsOf<Sale>(DateTime.UtcNow, String.Empty, String.Empty, SaleItemHandlerTestData.GenerateListSaleItem(0));

    public static List<Sale> GenerateListSales(int count)
    {
        var list = new List<Sale>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidSale());
        }
        return list;
    }
}
