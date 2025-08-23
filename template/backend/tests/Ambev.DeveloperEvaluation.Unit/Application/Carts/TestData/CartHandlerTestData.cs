using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

public static class CartHandlerTestData
{
    private static readonly Faker<Cart> createCartHandlerFaker = new Faker<Cart>()
        .RuleFor(u => u.Id, f => Guid.NewGuid())
        .RuleFor(u => u.Date, f => DateTime.UtcNow)
        .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.Products, f => CartItemHandlerTestData.GenerateListCartItem(f.Random.Int(1, 9999)) )
        .RuleFor(u => u.UserId, Guid.NewGuid());

    public static Cart GenerateValidCart()
    {
        return createCartHandlerFaker.Generate();
    }

    public static Cart GenerateInvalidCart()
    {
        var cart = Substitute.ForPartsOf<Cart>();
        cart.UserId = Guid.Empty;
        cart.Products = CartItemHandlerTestData.GenerateListCartItem(0);
        cart.CreatedAt = DateTime.Now;
        return cart;
    }

    public static List<Cart> GenerateListCarts(int count)
    {
        var list = new List<Cart>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidCart());
        }
        return list;
    }

    public static Cart GenerateValidCartWithProducts()
    {
        var faker = new Bogus.Faker();
        var products = new List<CartItem>();
        for (int i = 0; i < 2; i++)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(10, 100)
            };
            products.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = faker.Random.Int(1, 5)
            });
        }
        return new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = products
        };
    }
}
