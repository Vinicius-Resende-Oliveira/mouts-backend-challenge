using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _productRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="validator">The validator for DeleteSaleCommand</param>
    public DeleteSaleHandler(
        ISaleRepository productRepository,
        ILogger<DeleteSaleHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
        _logger.BeginScope("Begin DeleteSaleHandler");
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteSaleCommand");
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var success = await _productRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        _logger.LogInformation("Sale deleted {Id}", request.Id);
        return new DeleteSaleResponse { Success = true };
    }
}
