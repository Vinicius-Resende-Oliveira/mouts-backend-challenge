using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateRatingHandlerTestData
{
    private static readonly Faker f = new();

    public static BaseRating GenerateValidBaseRating()
        => new BaseRating(f.Random.Double(0.1, 5), f.Random.Int(1, 999999));


    public static Rating GenerateValidRating()
        => new Rating(f.Random.Double(0.1, 5), f.Random.Int(1, 999999));
}