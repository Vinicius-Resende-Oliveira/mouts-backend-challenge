using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData
{
    public class GetCartHandlerTestData
    {
        public static GetCartQuery GenerateValidCommand()
        {
            return new GetCartQuery(Guid.NewGuid());

        }

        public static GetCartQuery GenerateInvalidCommand()
        {
            return new GetCartQuery(Guid.Empty);
        }

        public static GetCartResult GenerateValidResult(Cart cart)
        {
            return new GetCartResult
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Date = cart.Date,
                Products = cart.Products?.Select(p => new BaseCartItem
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                }).ToList() ?? new List<BaseCartItem>()
            };
        }
    }
}
