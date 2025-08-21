using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Products.Common;

public class BaseProduct
{
    public string Title { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Image { get; set; } = String.Empty;
    public required BaseRating Rating { get; set; }
}

public record BaseRating(double Rate, int Count);