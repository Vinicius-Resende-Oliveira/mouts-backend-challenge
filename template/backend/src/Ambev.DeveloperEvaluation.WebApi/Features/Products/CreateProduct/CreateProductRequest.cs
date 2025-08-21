using Ambev.DeveloperEvaluation.Application.Products.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct
{
    public class CreateProductRequest
    {
        public string Title { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Category { get; set; } = String.Empty;
        public string Image { get; set; } = String.Empty;
        public required BaseRating Rating { get; set; }
    }
}
