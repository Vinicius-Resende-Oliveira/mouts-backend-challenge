using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateCart operation
    /// </summary>
    public UpdateCartProfile()
    {
        CreateMap<UpdateCartCommand, Cart>();
        CreateMap<Cart, UpdateCartResult>();
    }
}
