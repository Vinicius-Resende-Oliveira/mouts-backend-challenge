using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given a valid command When creating sale Then returns created sale result")]
    public async Task Handle_ValidRequest_ReturnsCreatedSaleResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCartWithProducts();
        var saleItems = new List<SaleItem>();
        foreach (var p in cart.Products)
            saleItems.Add(new SaleItem(p.ProductId, p.Quantity, p.Product?.Price ?? 0));

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = 123,
            SaleDate = command.SaleDate,
            Customer = command.Customer,
            Branch = command.Branch,
            IsCancelled = false,
            Items = saleItems
        };
        var createdSale = sale;
        var result = CreateSaleHandlerTestData.GenerateValidResult(sale);

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>()).Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // Act
        var createResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        createResult.Should().NotBeNull();
        createResult.Customer.Should().Be(command.Customer);
        createResult.Branch.Should().Be(command.Branch);
        await _saleRepository.Received(1).CreateAsync(sale, Arg.Any<CancellationToken>());
        await _cartRepository.Received(1).GetByIdAsync(command.CartId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a non-existent cart When creating sale Then throws not found exception")]
    public async Task Handle_CartNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cart with ID {command.CartId} not found");
    }

    [Fact(DisplayName = "Given a valid command When creating sale Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCartWithProducts();
        var sale = new Sale();

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(CreateSaleHandlerTestData.GenerateValidResult(sale));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<Sale>(command);
    }
}