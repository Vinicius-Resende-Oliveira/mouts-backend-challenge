using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, PaginatedList<GetProductResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListProductsQueryHandler> _logger;

    public ListProductsQueryHandler(IProductRepository productRepository, IMapper mapper, ILogger<ListProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin ListProductsQueryHandler");
    }

    public async Task<PaginatedList<GetProductResult>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ListProductsQuery");
        var query = _productRepository.GetAll(cancellationToken).AsNoTracking();

        query = _productRepository.Filter(query, nameof(GetProductResult.Title), request.Title);
        query = _productRepository.Filter(query, nameof(GetProductResult.Description), request.Description);
        query = _productRepository.Filter(query, nameof(GetProductResult.Category), request.Category);
        query = _productRepository.Filter(query, nameof(GetProductResult.Image), request.Image);
        query = _productRepository.FilterRange(query, nameof(GetProductResult.Price), request.MinPrice, request.MaxPrice);

        _logger.LogInformation("Filter query");
        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orders = request.Order
                .Trim().Trim('"', '\'')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(part =>
                {
                    var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = tokens[0];
                    var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                    return (field, desc);
                })
                .Where(t => !string.IsNullOrWhiteSpace(t.field))
                .ToArray();

            query = _productRepository.OrderByFields(query, orders);
        }
        _logger.LogInformation("Order query");

        var getProductList = _mapper.ProjectTo<GetProductResult>(query);
        _logger.LogInformation("Mapped GetProductResult");

        return ListProductsResponse.Create(getProductList, request.Page, request.Size);
    }
}
