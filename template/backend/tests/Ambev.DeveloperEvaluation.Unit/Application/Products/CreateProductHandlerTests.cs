using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contém testes unitários para <see cref="CreateProductHandler"/>
/// </summary>
public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateProductHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid data When create product Then return success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();
        var result = CreateProductHandlerTestData.GenerateValidResult();

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(result);

        // Act
        var createProductResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        createProductResult.Should().NotBeNull();
        createProductResult.Id.Should().Be(result.Id);
        createProductResult.Title.Should().Be(result.Title);
        await _productRepository.Received(1).CreateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid user data When creating user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When handling Then maps command to user entity")]
    public async Task Handle_ValidRequest_MapsCommandToProduct()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(CreateProductHandlerTestData.GenerateValidResult());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<Product>(Arg.Is<CreateProductCommand>(c =>
            c.Title == command.Title &&
            c.Price == command.Price &&
            c.Description == command.Description &&
            c.Category == command.Category &&
            c.Image == command.Image));
    }

    [Fact(DisplayName = "Given valid command When manipulate Then persist product and save changes")]
    public async Task Handle_ValidRequest_PersistsProductAndSavesChanges()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(CreateProductHandlerTestData.GenerateValidResult());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _productRepository.Received(1).CreateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given failed to persist product When handle Then throws exception")]
    public async Task Handle_PersistFails_ThrowsException()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, Arg.Any<CancellationToken>())
            .Throws(new Exception("Erro ao persistir produto"));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao persistir produto");
    }
}