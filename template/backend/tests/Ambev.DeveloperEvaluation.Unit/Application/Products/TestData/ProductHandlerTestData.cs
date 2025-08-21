using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;

public static class ProductHandlerTestData
{
    private static readonly Faker<Product> createProductHandlerFaker = new Faker<Product>()
        .RuleFor(u => u.Id, Guid.NewGuid())
        .RuleFor(u => u.Rating, f => CreateRatingHandlerTestData.GenerateValidRating())
        .RuleFor(u => u.Title, f => f.Commerce.ProductName())
        .RuleFor(u => u.Description, f => f.Lorem.Word())
        .RuleFor(u => u.Category, f => f.Commerce.Categories(1).First())
        .RuleFor(u => u.Image, f => f.Internet.Url())
        .RuleFor(u => u.Price, f => f.Random.Decimal())
        .RuleFor(u => u.CreatedAt, f => DateTime.Now);

    public static Product GenerateValidProduct()
    {
        return createProductHandlerFaker.Generate();
    }

    public static Product GenerateInvalidProduct()
    {
        var product = Substitute.ForPartsOf<Product>();
        product.Title = "";
        product.Price = -1;
        product.Description = "";
        product.Category = "";
        product.Image = "";
        product.Rating = new Rating(0, 0);
        product.CreatedAt = DateTime.Now;
        return product;
    }

    public static List<Product> GenerateListProducts(int count)
    {
        var list = new List<Product>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidProduct());
        }
        return list;
    }
}
