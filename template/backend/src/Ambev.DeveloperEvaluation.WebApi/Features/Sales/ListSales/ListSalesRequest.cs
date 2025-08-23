using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public class ListSalesRequest : PaginatedRequest
{
    public int? SaleNumber { get; set; }
    public bool? IsCancelled { get; set; }
    public string Customer { get; set; } = String.Empty;
    public string Branch { get; set; } = String.Empty;

    [JsonPropertyName("_minSaleDate")]
    public DateTime? MinSaleDate { get; set; }
    [JsonPropertyName("_maxSaleDate")]
    public DateTime? MaxSaleDate { get; set; }
}
