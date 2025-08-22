using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CartItemHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid BaseCartItem.
    /// </summary>
    private static readonly Faker<BaseCartItem> createBaseCartItemCommandHandlerFaker = new Faker<BaseCartItem>()
        .RuleFor(u => u.ProductId, f => Guid.NewGuid())
        .RuleFor(u => u.Quantity, f => f.Random.Int(0,999999));

    private static readonly Faker<CartItem> createCartItemCommandHandlerFaker = new Faker<CartItem>()
        .RuleFor(u => u.ProductId, f => Guid.NewGuid())
        .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
        .RuleFor(u => u.Quantity, f => f.Random.Int(0, 999999));

    /// <summary>
    /// Generates a valid Cart Item entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Cart Item entity with randomly generated data.</returns>
    public static BaseCartItem GenerateValidBaseCartItem()
    {
        return createBaseCartItemCommandHandlerFaker.Generate();
    }

    public static CartItem GenerateValidCartItem()
    {
        return createCartItemCommandHandlerFaker.Generate();
    }

    public static BaseCartItem GenerateInvalidBaseCartItem()
    {
        var baseCartItem = Substitute.ForPartsOf<BaseCartItem>();
        baseCartItem.ProductId = Guid.Empty;
        baseCartItem.Quantity = 0;

        return baseCartItem;
    }

    public static List<CartItem> GenerateListCartItem(int count = 0)
    {
        var list = new List<CartItem>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidCartItem());
        }
        return list;
    }

    public static List<BaseCartItem> GenerateListBaseCartItem(int count = 0)
    {
        var list = new List<BaseCartItem>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateValidBaseCartItem());
        }
        return list;
    }
}
