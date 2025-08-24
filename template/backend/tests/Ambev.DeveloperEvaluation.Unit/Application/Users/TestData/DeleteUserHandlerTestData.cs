using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData
{
    public class DeleteUserHandlerTestData
    {
        public static DeleteUserCommand GenerateValidCommand()
        {
            return new DeleteUserCommand(Guid.NewGuid());

        }

        public static DeleteUserCommand GenerateInvalidCommand()
        {
            return new DeleteUserCommand(Guid.Empty);
        }

        public static DeleteUserResponse GenerateValidResult()
        {
            return new DeleteUserResponse { Success = true };
        }
    }
}
