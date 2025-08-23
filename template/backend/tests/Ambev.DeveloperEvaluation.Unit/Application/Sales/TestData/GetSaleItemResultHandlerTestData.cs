using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class GetSaleItemResultHandlerTestData
{
    private static readonly Faker<GetSaleItemResult> createGetSaleItemResultCommandHandlerFaker = new Faker<GetSaleItemResult>()
        .RuleFor(u => u.ProductId, f => Guid.NewGuid())
        .RuleFor(u => u.Discount, f => f.Random.Decimal(0, 1))
        .RuleFor(u => u.UnitPrice, f => f.Random.Decimal(1, 999))
        .RuleFor(u => u.TotalValue, f => f.Random.Decimal(1, 9999))
        .RuleFor(u => u.Quantity, f => f.Random.Int(1, 21));

    public static GetSaleItemResult GenerateValidSaleItem()
        => createGetSaleItemResultCommandHandlerFaker.Generate();

    public static GetSaleItemResult GenerateInvalidSaleItem()
        => Substitute.ForPartsOf<GetSaleItemResult>(Guid.Empty, 0, 0);

    public static List<GetSaleItemResult> GenerateListSaleItem(int count = 0)
    {
        var list = new List<GetSaleItemResult>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidSaleItem());
        }
        return list;
    }
}
