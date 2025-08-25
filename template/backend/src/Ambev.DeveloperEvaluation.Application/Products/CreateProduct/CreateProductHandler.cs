using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateProductHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for CreateProductCommand</param>
    public CreateProductHandler(IProductRepository userRepository, IMapper mapper, ILogger<CreateProductHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin CreateProductHandler");
    }

    /// <summary>
    /// Handles the CreateProductCommand request
    /// </summary>
    /// <param name="command">The CreateProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user details</returns>
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateProductCommand");
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        _logger.LogInformation("Is Valid command");
        var user = _mapper.Map<Product>(command);

        var createdProduct = await _userRepository.CreateAsync(user, cancellationToken);
        _logger.LogInformation("Product created {Id}", createdProduct.Id);

        var result = _mapper.Map<CreateProductResult>(createdProduct);
        return result;
    }
}
