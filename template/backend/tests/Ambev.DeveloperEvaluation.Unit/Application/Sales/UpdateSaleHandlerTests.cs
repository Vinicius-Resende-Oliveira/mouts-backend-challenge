using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _productRepository, _mapper);
    }

    [Fact(DisplayName = "Given a valid command When updating sale Then returns updated sale result")]
    public async Task Handle_ValidRequest_ReturnsUpdatedSaleResult()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleHandlerTestData.GenerateValidSale();
        sale.IsCancelled = false;
        var products = command.Items.Select(i => new Product { Id = i.ProductId, Price = 10m }).ToList();
        var saleItems = command.Items.Select(i => new SaleItem(i.ProductId, i.Quantity, 10m)).ToList();
        var result = UpdateSaleHandlerTestData.GenerateValidResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(products);
        _mapper.Map(command, sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        // Act
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        updateResult.Should().NotBeNull();
        _saleRepository.Received(1).Update(sale);
        await _saleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a non-existent sale When updating sale Then throws not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with id {command.Id} not found");
    }

    [Fact(DisplayName = "Given a cancelled sale When updating sale Then throws invalid operation exception")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleHandlerTestData.GenerateValidSale();
        sale.IsCancelled = true;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale");
    }

    [Fact(DisplayName = "Given a sale with non-existent products When updating sale Then throws not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleHandlerTestData.GenerateValidSale();
        sale.IsCancelled = false;
        // Simula que nenhum produto foi encontrado
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<Product>());

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("One or more products in the sale items do not exist");
    }

    [Fact(DisplayName = "Given a valid command When updating sale Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleHandlerTestData.GenerateValidSale();
        sale.IsCancelled = false;
        var products = command.Items.Select(i => new Product { Id = i.ProductId, Price = 10m }).ToList();
        var result = UpdateSaleHandlerTestData.GenerateValidResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(products);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map(command, sale);
    }
}