using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public class ListCartsRequest : PaginatedRequest
{
    public Guid? UserId { get; set; }
    [JsonPropertyName("_minDate")]
    public DateTime? MinDate { get; set; }
    [JsonPropertyName("_maxDate")]
    public DateTime? MaxDate { get; set; }
}
