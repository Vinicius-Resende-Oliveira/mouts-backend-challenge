using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsCommand, PaginatedList<GetProductResult>>
{
    private readonly IProductRepository _userRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetProductResult>> Handle(ListProductsCommand request, CancellationToken cancellationToken)
    {
        var query = _userRepository.GetAll(cancellationToken).AsNoTracking();

        query = _userRepository.Filter(query, nameof(request.Title), request.Title);
        query = _userRepository.Filter(query, nameof(request.Description), request.Description);
        query = _userRepository.Filter(query, nameof(request.Category), request.Category);
        query = _userRepository.Filter(query, nameof(request.Image), request.Image);
        query = _userRepository.FilterRange(query, nameof(request.MinPrice), request.MinPrice, request.MaxPrice);

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

            query = _userRepository.OrderByFields(query, orders);
        }

        var getProductList = _mapper.ProjectTo<GetProductResult>(query);
        return await ListProductsResponse.CreateAsync(getProductList, request.Page, request.Size);
    }
}
