using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData
{
    public class GetProductHandlerTestData
    {
        public static GetProductCommand GenerateValidCommand()
        {
            return new GetProductCommand(Guid.NewGuid());

        }

        public static GetProductCommand GenerateInvalidCommand()
        {
            return new GetProductCommand(Guid.Empty);
        }

        public static GetProductResult GenerateValidResult(Product product)
        {
            return new GetProductResult
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
                Image = product.Image,
                Rating = new BaseRating(product.Rating.Rate, product.Rating.Count)
            };
        }
    }
}
