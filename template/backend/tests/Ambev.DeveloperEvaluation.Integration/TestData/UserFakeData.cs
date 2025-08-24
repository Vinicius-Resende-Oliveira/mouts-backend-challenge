using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData
{
    public static class UserFakeData
    {
        public static Faker<User> GetUserFaker()
        {
            return new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Name, f => new Name(f.Name.FirstName(), f.Name.LastName()))
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Phone, f => f.Random.Int(11, 99).ToString() + f.Random.ReplaceNumbers("99#######"))
                .RuleFor(u => u.Password, f => f.Internet.Password(15, true, @"^(?=\\S{8,128}$)(?=.*\\p{Lu})(?=.*\\p{Ll})(?=.*\\p{Nd})(?=.*[^\\p{L}\\p{N}])\\S+$", "@123Senha"))
                .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Admin, UserRole.Manager, UserRole.Customer))
                .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Inactive, UserStatus.Suspended))
                .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
                .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow);
        }

        public static User GenerateValidUser()
        {
            return GetUserFaker().Generate();
        }

        public static User GenerateValidUserWithEmail(string email)
        {
            var faker = GetUserFaker();
            faker.RuleFor(u => u.Email, _ => email);
            return faker.Generate();
        }
    }
}
