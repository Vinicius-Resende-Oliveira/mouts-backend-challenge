using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsQueryHandler : IRequestHandler<ListCartsQuery, PaginatedList<GetCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ListCartsQueryHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetCartResult>> Handle(ListCartsQuery request, CancellationToken cancellationToken)
    {
        var query = _cartRepository.GetAll(cancellationToken).AsNoTracking();

        query = _cartRepository.Filter(query, nameof(GetCartResult.UserId), request.UserId.ToString());
        query = _cartRepository.FilterRange(query, nameof(GetCartResult.Date), request.MinDate, request.MaxDate);

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

        var getCartList = _mapper.ProjectTo<GetCartResult>(query);
        return ListCartsResponse.Create(getCartList, request.Page, request.Size);
    }
}
