using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts
{
    public class ListCartsProfile : Profile
    {
        public ListCartsProfile()
        {
            CreateMap<ListCartsRequest, ListCartsQuery>();
            CreateMap<GetCartResult, GetCartResponse>();
        }
    }
}
