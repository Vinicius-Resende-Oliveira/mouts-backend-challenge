using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="productRepository">The product repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for UpdateSaleCommand</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IProductRepository productRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin UpdateSaleHandler");
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateSaleCommand");
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        _logger.LogInformation("Is Valid command");

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with id {command.Id} not found");
        _logger.LogInformation("Get sale {id}", sale.Id);

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");
        
        _logger.LogInformation("Sale is not Cancelled {id}", sale.Id);

        var Items = await _productRepository.GetByIdsAsync(command.Items.Select(i => i.ProductId).ToList(), cancellationToken);

        if (Items.Count != command.Items.Count)
            throw new KeyNotFoundException("One or more products in the sale items do not exist");
        _logger.LogInformation("Items sale exists {id}", sale.Id);

        var saleItems = command.Items
            .Select(item => 
                new SaleItem(
                    item.ProductId, 
                    item.Quantity, 
                    Items.First(p => p.Id == item.ProductId).Price,
                    true
                )
            ).ToList();

        _mapper.Map(command, sale);
        sale.SetItems(saleItems);
        sale.Update();
        _logger.LogInformation("Items sale seted {id}", sale.Id);

        _saleRepository.Update(sale);
        await _saleRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated sale {id}", sale.Id);

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}