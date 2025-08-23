using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesResponse : PaginatedList<GetSaleResult>
{
    public ListSalesResponse(List<GetSaleResult> items, int count, int pageNumber, int pageSize)
        : base(items, count, pageNumber, pageSize)
    {
    }
}
