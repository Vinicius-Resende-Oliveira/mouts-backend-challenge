using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsQueryHandler : IRequestHandler<ListCartsQuery, PaginatedList<GetCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListCartsQueryHandler> _logger;

    public ListCartsQueryHandler(ICartRepository cartRepository, IMapper mapper, ILogger<ListCartsQueryHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin ListCartsQueryHandler");
    }

    public async Task<PaginatedList<GetCartResult>> Handle(ListCartsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ListCartsQuery");
        var query = _cartRepository.GetAll(cancellationToken).AsNoTracking();

        query = _cartRepository.Filter(query, nameof(GetCartResult.UserId), request.UserId.ToString());
        query = _cartRepository.FilterRange(query, nameof(GetCartResult.Date), request.MinDate, request.MaxDate);

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

            query = _cartRepository.OrderByFields(query, orders);
        }
        _logger.LogInformation("Order query");

        var getCartList = _mapper.ProjectTo<GetCartResult>(query);
        _logger.LogInformation("Mapped GetCartResult");

        return ListCartsResponse.Create(getCartList, request.Page, request.Size);
    }
}
