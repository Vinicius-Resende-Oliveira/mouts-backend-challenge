using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData
{
    public class GetUserHandlerTestData
    {
        public static GetUserQuery GenerateValidCommand()
        {
            return new GetUserQuery(Guid.NewGuid());

        }

        public static GetUserQuery GenerateInvalidCommand()
        {
            return new GetUserQuery(Guid.Empty);
        }

        public static GetUserResult GenerateValidResult(User user)
        {
            return new GetUserResult
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status,
                Name = new BaseName(user.Name.FirstName, user.Name.LastName)
            };
        }
    }
}
