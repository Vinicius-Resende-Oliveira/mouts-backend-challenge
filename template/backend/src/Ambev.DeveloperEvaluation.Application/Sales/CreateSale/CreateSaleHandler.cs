using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for CreateSaleCommand</param>
    public CreateSaleHandler(ISaleRepository saleRepository, ICartRepository cartRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin CreateSaleHandler");
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateSaleCommand");
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var cart = await _cartRepository.GetByIdAsync(command.CartId, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {command.CartId} not found");
        _logger.LogInformation("Get cart {id}", cart.Id);

        var saleItems = cart.Products.Select(p => new SaleItem(p.ProductId, p.Quantity, p.Product.Price)).ToList();

        var sale = _mapper.Map<Sale>(command);
        sale.SetItems(saleItems);
        _logger.LogInformation("Items sale seted {id}, quantity {quantity}", sale.Id, saleItems.Count);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        _logger.LogInformation("Cart created {Id}", createdSale.Id);
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
