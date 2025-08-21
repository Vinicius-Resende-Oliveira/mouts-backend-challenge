using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData
{
    public class DeleteProductHandlerTestData
    {
        public static DeleteProductCommand GenerateValidCommand()
        {
            return new DeleteProductCommand(Guid.NewGuid());

        }

        public static DeleteProductCommand GenerateInvalidCommand()
        {
            return new DeleteProductCommand(Guid.Empty);
        }

        public static DeleteProductResponse GenerateValidResult()
        {
            return new DeleteProductResponse { Success = true };
        }
    }
}
