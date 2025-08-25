using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for UpdateCartCommand</param>
    public UpdateCartHandler(ICartRepository cartRepository, IMapper mapper, ILogger<UpdateCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin UpdateCartHandler");
    }

    /// <summary>
    /// Handles the UpdateCartCommand request
    /// </summary>
    /// <param name="command">The UpdateCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created cart details</returns>
    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCartCommand");
        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var cart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Cart with id {command.Id} not found");
        
        _logger.LogInformation("Get cart {id}", cart.Id);

        _mapper.Map(command, cart);
        cart.Update();

        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated cart {id}", cart.Id);

        return _mapper.Map<UpdateCartResult>(cart);
    }
}