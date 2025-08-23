using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData
{
    public class DeleteSaleHandlerTestData
    {
        public static DeleteSaleCommand GenerateValidCommand()
        {
            return new DeleteSaleCommand(Guid.NewGuid());

        }

        public static DeleteSaleCommand GenerateInvalidCommand()
        {
            return new DeleteSaleCommand(Guid.Empty);
        }

        public static DeleteSaleResponse GenerateValidResult()
        {
            return new DeleteSaleResponse { Success = true };
        }
    }
}
