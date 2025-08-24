using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class UpdateUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid User entities.
    /// </summary>
    private static readonly Faker<UpdateUserCommand> createUserCommandHandlerFaker = new Faker<UpdateUserCommand>()
        .RuleFor(u => u.Id, Guid.NewGuid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Name, f => CreateNameHandlerTestData.GenerateValidCommand())
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin));

    /// <summary>
    /// Generates a valid User entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid User entity with randomly generated data.</returns>
    public static UpdateUserCommand GenerateValidCommand()
    {
        return createUserCommandHandlerFaker.Generate();
    }

    public static UpdateUserCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<UpdateUserCommand>();
        command.Id = Guid.Empty;
        command.Username = String.Empty;
        command.Password = String.Empty;
        command.Email = String.Empty;
        command.Phone  = String.Empty;
        command.Status = UserStatus.Unknown;
        command.Role = UserRole.None;
        return command;
    }

    public static UpdateUserResult GenerateValidResult(Guid id, UpdateUserCommand command)
    {
        return new UpdateUserResult
        {
            Id = id,
            Name = command.Name,
            Username = command.Username,
            Email = command.Email,
            Phone  = command.Phone ,
            Status = command.Status,
            Role = command.Role
        };
    }
}
