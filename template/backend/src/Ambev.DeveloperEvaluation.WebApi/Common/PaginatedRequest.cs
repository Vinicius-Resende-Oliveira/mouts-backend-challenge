using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Common
{
    public class PaginatedRequest
    {
        [JsonPropertyName("_page")] 
        public int Page { get; set; } = 1;
        [JsonPropertyName("_size")] 
        public int Size { get; set; } = 10;
        [JsonPropertyName("_order")] 
        public string? Order { get; set; } = null;
    }
}
