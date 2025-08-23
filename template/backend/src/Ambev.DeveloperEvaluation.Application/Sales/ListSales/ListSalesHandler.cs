using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, PaginatedList<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetSaleResult>> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var query = _saleRepository.GetAll(cancellationToken).AsNoTracking();

        query = _saleRepository.Filter(query, nameof(GetSaleResult.Customer), request.Customer);
        query = _saleRepository.Filter(query, nameof(GetSaleResult.Branch), request.Branch);
        query = _saleRepository.Filter(query, nameof(GetSaleResult.SaleNumber), request.SaleNumber.ToString());
        query = _saleRepository.BoolFilter(query, nameof(GetSaleResult.IsCancelled), request.IsCancelled);
        query = _saleRepository.FilterRange(query, nameof(GetSaleResult.SaleDate), request.MinSaleDate, request.MaxSaleDate);

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

            query = _saleRepository.OrderByFields(query, orders);
        }

        var getSaleList = _mapper.ProjectTo<GetSaleResult>(query);
        return ListSalesResponse.Create(getSaleList, request.Page, request.Size);
    }
}
