using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class UpdateCartHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Cart entities.
    /// </summary>
    private static readonly Faker<UpdateCartCommand> createCartCommandHandlerFaker = new Faker<UpdateCartCommand>()
        .RuleFor(u => u.Id, Guid.NewGuid())
        .RuleFor(u => u.Date, f => DateTime.UtcNow)
        .RuleFor(u => u.Products, f => CartItemHandlerTestData.GenerateListBaseCartItem(f.Random.Int(1, 99999)))
        .RuleFor(u => u.UserId, Guid.NewGuid());

    /// <summary>
    /// Generates a valid Cart entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Cart entity with randomly generated data.</returns>
    public static UpdateCartCommand GenerateValidCommand()
    {
        return createCartCommandHandlerFaker.Generate();
    }

    public static UpdateCartCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<UpdateCartCommand>();
        command.UserId = Guid.Empty;
        command.Products = CartItemHandlerTestData.GenerateListBaseCartItem(0);
        return command;
    }

    public static UpdateCartResult GenerateValidResult(Guid id, UpdateCartCommand command)
    {
        return new UpdateCartResult
        {
            Id = id,
            Date = command.Date,
            Products = command.Products,
            UserId = command.UserId
        };
    }
}
