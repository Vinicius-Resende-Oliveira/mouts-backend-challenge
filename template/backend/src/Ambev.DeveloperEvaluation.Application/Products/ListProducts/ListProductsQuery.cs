using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsQuery : PaginatedListRequest, IRequest<PaginatedList<GetProductResult>>
{
    public string Title { get; set; } = String.Empty;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Description { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Image { get; set; } = String.Empty;
}
