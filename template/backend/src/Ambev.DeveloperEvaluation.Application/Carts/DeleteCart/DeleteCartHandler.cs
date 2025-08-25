using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Handler for processing DeleteCartCommand requests
/// </summary>
public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<DeleteCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="validator">The validator for DeleteCartCommand</param>
    public DeleteCartHandler(
        ICartRepository cartRepository, 
        ILogger<DeleteCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _logger = logger;
        _logger.BeginScope("Begin DeleteCartHandler");
    }

    /// <summary>
    /// Handles the DeleteCartCommand request
    /// </summary>
    /// <param name="request">The DeleteCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteCartResponse> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteCartCommand");
        var validator = new DeleteCartValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var success = await _cartRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Cart with ID {request.Id} not found");

        _logger.LogInformation("Cart deleted {Id}", request.Id);
        return new DeleteCartResponse { Success = true };
    }
}
