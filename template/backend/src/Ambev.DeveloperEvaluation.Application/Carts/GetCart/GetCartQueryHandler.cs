using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Handler for processing GetCartCommand requests
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, GetCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCartQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for GetCartCommand</param>
    public GetCartQueryHandler(
        ICartRepository cartRepository,
        IMapper mapper, 
        ILogger<GetCartQueryHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin GetCartQueryHandler");
    }

    /// <summary>
    /// Handles the GetCartCommand request
    /// </summary>
    /// <param name="request">The GetCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cart details if found</returns>
    public async Task<GetCartResult> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCartQuery");
        var validator = new GetCartQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);

        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {request.Id} not found");

        _logger.LogInformation("Cart get {Id}", request.Id);
        return _mapper.Map<GetCartResult>(cart);
    }
}
