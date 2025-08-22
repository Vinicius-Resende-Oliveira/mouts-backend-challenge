using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData
{
    public class GetCartHandlerTestData
    {
        public static GetCartCommand GenerateValidCommand()
        {
            return new GetCartCommand(Guid.NewGuid());

        }

        public static GetCartCommand GenerateInvalidCommand()
        {
            return new GetCartCommand(Guid.Empty);
        }

        public static GetCartResult GenerateValidResult(Cart product)
        {
            return new GetCartResult
            {
                Id = product.Id,
                UserId = product.UserId,
                Date = product.Date,
                Products = product.Products?.Select(p => new BaseCartItem
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                }).ToList() ?? new List<BaseCartItem>()
            };
        }
    }
}
