using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesCommand : PaginatedListRequest, IRequest<PaginatedList<GetSaleResult>>
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
