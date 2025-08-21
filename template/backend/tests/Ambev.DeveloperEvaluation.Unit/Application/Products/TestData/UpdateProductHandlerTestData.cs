using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class UpdateProductHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Product entities.
    /// </summary>
    private static readonly Faker<UpdateProductCommand> createProductCommandHandlerFaker = new Faker<UpdateProductCommand>()
        .RuleFor(u => u.Id, f => Guid.NewGuid())
        .RuleFor(u => u.Rating, f => CreateRatingHandlerTestData.GenerateValidBaseRating())
        .RuleFor(u => u.Title, f => f.Commerce.ProductName())
        .RuleFor(u => u.Description, f => f.Lorem.Word())
        .RuleFor(u => u.Category, f => f.Commerce.Categories(1).First())
        .RuleFor(u => u.Image, f => f.Internet.Url())
        .RuleFor(u => u.Price, f => f.Random.Decimal());

    /// <summary>
    /// Generates a valid Product entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Product entity with randomly generated data.</returns>
    public static UpdateProductCommand GenerateValidCommand()
    {
        return createProductCommandHandlerFaker.Generate();
    }

    public static UpdateProductCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<UpdateProductCommand>();
        command.Id = Guid.Empty;
        command.Title = "";
        command.Price = -1;
        command.Description = "";
        command.Category = "";
        command.Image = "";
        command.Rating = new BaseRating(0, 0);

        return command;
    }

    public static UpdateProductResult GenerateValidResult(Guid id, UpdateProductCommand command)
    {
        return new UpdateProductResult
        {
            Id = id,
            Title = command.Title,
            Price = command.Price,
            Description = command.Description,
            Category = command.Category,
            Image = command.Image,
            Rating = command.Rating
        };
    }
}
