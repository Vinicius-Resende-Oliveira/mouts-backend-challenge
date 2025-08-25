using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesQueryHandler : IRequestHandler<ListSalesQuery, PaginatedList<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListSalesQueryHandler> _logger;

    public ListSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<ListSalesQueryHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin ListSalesQueryHandler");
    }

    public async Task<PaginatedList<GetSaleResult>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ListSalesQuery");
        var query = _saleRepository.GetAll(cancellationToken).AsNoTracking();

        query = _saleRepository.Filter(query, nameof(GetSaleResult.Customer), request.Customer);
        query = _saleRepository.Filter(query, nameof(GetSaleResult.Branch), request.Branch);
        query = _saleRepository.Filter(query, nameof(GetSaleResult.SaleNumber), request.SaleNumber.ToString());
        query = _saleRepository.BoolFilter(query, nameof(GetSaleResult.IsCancelled), request.IsCancelled);
        query = _saleRepository.FilterRange(query, nameof(GetSaleResult.SaleDate), request.MinSaleDate, request.MaxSaleDate);

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

            query = _saleRepository.OrderByFields(query, orders);
        }
        _logger.LogInformation("Order query");

        var getSaleList = _mapper.ProjectTo<GetSaleResult>(query);
        _logger.LogInformation("Mapped GetSaleResult");
        return ListSalesResponse.Create(getSaleList, request.Page, request.Size);
    }
}
