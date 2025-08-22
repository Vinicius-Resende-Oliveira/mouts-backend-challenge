using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateCart operation
    /// </summary>
    public CreateCartProfile()
    {
        CreateMap<CreateCartCommand, Cart>();
        CreateMap<Cart, CreateCartResult>();
    }
}