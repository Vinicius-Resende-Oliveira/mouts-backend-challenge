using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateSale feature
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<GetSaleItemResult, UpdateSaleItemResponse>();
    }
}
