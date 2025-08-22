using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData
{
    public class DeleteCartHandlerTestData
    {
        public static DeleteCartCommand GenerateValidCommand()
        {
            return new DeleteCartCommand(Guid.NewGuid());

        }

        public static DeleteCartCommand GenerateInvalidCommand()
        {
            return new DeleteCartCommand(Guid.Empty);
        }

        public static DeleteCartResponse GenerateValidResult()
        {
            return new DeleteCartResponse { Success = true };
        }
    }
}
