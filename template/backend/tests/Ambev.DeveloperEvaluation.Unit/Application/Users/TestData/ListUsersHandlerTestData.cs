using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;

public static class ListUsersHandlerTestData
{
    public static ListUsersQuery GenerateValidCommand()
    {
        return new ListUsersQuery
        {
            Page = 1,
            Size = 10,
            Username = null,
            Email = null,
            Phone = null,
            Role = UserRole.None,
            Status = UserStatus.Unknown,
            Order = null
        };
    }

    public static List<User> GenerateUserList()
    {
        return new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                Phone = "+5511999999999",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                Name = new Name("Test", "User")
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "anotheruser",
                Email = "anotheruser@example.com",
                Phone = "+5511988888888",
                Role = UserRole.Manager,
                Status = UserStatus.Inactive,
                Name = new Name("Another", "User")
            }
        };
    }
}
