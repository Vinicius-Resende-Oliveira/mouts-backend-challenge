using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData
{
    public class GetSaleHandlerTestData
    {
        public static GetSaleQuery GenerateValidCommand()
        {
            return new GetSaleQuery(Guid.NewGuid());

        }

        public static GetSaleQuery GenerateInvalidCommand()
        {
            return new GetSaleQuery(Guid.Empty);
        }

        public static GetSaleResult GenerateValidResult(Sale sale)
        {
            return new GetSaleResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                Customer = sale.Customer,
                Branch = sale.Branch,
                IsCancelled = sale.IsCancelled,
                Items = sale.Items?.Select(i => new GetSaleItemResult
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalValue = i.TotalValue,
                    Discount = i.Discount
                }).ToList() ?? new List<GetSaleItemResult>()
            };
        }
    }
}
