using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartHandler : IRequestHandler<CreateCartCommand, CreateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The log</param>
    /// <param name="validator">The validator for CreateCartCommand</param>
    public CreateCartHandler(ICartRepository cartRepository, IMapper mapper, ILogger<CreateCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin CreateCartHandler");
    }

    /// <summary>
    /// Handles the CreateCartCommand request
    /// </summary>
    /// <param name="command">The CreateCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created cart details</returns>
    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCartCommand");
        var validator = new CreateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        _logger.LogInformation("Is Valid command");
        var cart = _mapper.Map<Cart>(command);

        var createdCart = await _cartRepository.CreateAsync(cart, cancellationToken);

        _logger.LogInformation("Cart created {Id}", createdCart.Id);
        var result = _mapper.Map<CreateCartResult>(createdCart);
        return result;
    }
}
