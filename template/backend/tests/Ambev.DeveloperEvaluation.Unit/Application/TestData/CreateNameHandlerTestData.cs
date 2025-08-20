using Ambev.DeveloperEvaluation.Application.Users.Common;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateNameHandlerTestData
{
    private static readonly Faker f = new();

    public static BaseName GenerateValidCommand()
        => new BaseName(f.Name.FirstName(), f.Name.LastName());
}