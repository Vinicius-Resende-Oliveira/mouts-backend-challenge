using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsRequest : PaginatedRequest
{
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Image { get; set; } = String.Empty;
    [JsonPropertyName("_minPrice")]
    public decimal? MinPrice { get; set; }
    [JsonPropertyName("_maxPrice")]
    public decimal? MaxPrice { get; set; }
}
