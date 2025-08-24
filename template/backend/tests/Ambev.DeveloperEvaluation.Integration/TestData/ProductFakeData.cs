using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData
{
    public static class ProductFakeData
    {
        public static Faker<Product> GetProductFaker()
        {
            return new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Rating, f => new Rating(f.Random.Double(0, 5), f.Random.Int(0, 1000)))
                .RuleFor(p => p.CreatedAt, f => DateTime.UtcNow)
                .RuleFor(p => p.UpdatedAt, f => DateTime.UtcNow);
        }

        public static Product GenerateValidProduct()
        {
            return GetProductFaker().Generate();
        }
    }
}
