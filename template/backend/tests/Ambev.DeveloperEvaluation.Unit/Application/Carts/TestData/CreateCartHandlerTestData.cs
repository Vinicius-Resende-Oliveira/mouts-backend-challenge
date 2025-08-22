using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateCartHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Cart entities.
    /// </summary>
    private static readonly Faker<CreateCartCommand> createCartCommandHandlerFaker = new Faker<CreateCartCommand>()
        .RuleFor(u => u.Date, f => DateTime.UtcNow)
        .RuleFor(u => u.Products, f => CartItemHandlerTestData.GenerateListBaseCartItem(f.Random.Int(1, 9999)))
        .RuleFor(u => u.UserId, Guid.NewGuid());

    private static readonly Faker<CreateCartResult> createCartResultHandlerFaker = new Faker<CreateCartResult>()
        .RuleFor(u => u.Date, f => DateTime.UtcNow)
        .RuleFor(u => u.Products, f => CartItemHandlerTestData.GenerateListBaseCartItem(f.Random.Int(1, 9999)))
        .RuleFor(u => u.UserId, Guid.NewGuid());

    /// <summary>
    /// Generates a valid Cart entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Cart entity with randomly generated data.</returns>
    public static CreateCartCommand GenerateValidCommand()
    {
        return createCartCommandHandlerFaker.Generate();
    }

    public static CreateCartCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<CreateCartCommand>();
        command.UserId = Guid.Empty;
        command.Products = CartItemHandlerTestData.GenerateListBaseCartItem(0);
        return command;
    }

    public static CreateCartResult GenerateValidResult()
    {
        return createCartResultHandlerFaker.Generate();
    }
}
