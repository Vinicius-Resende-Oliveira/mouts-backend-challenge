using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class ListProductsHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ListProductsQueryHandler _handler;

    public ListProductsHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListProductsQueryHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given filters and pagination When listing products Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var products = ProductHandlerTestData.GenerateListProducts(10);
        var results = products.Select(GetProductHandlerTestData.GenerateValidResult).ToList();
        var queryable = products.AsQueryable();

        var command = new ListProductsQuery
        {
            Page = 1,
            Size = 10,
            Title = null,
            Description = null,
            Category = null,
            Image = null,
            MinPrice = null,
            MaxPrice = null,
            Order = null
        };

        _productRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _productRepository.Filter(Arg.Any<IQueryable<Product>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(queryable);
        _productRepository.FilterRange(Arg.Any<IQueryable<Product>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(queryable);
        
        // Simula o ProjectTo do AutoMapper
        _mapper.ProjectTo<GetProductResult>(Arg.Any<IQueryable<Product>>()).Returns(results.AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().HaveCount(10);
        paginatedList.Select(p => p.Id).Should().BeEquivalentTo(results.Select(r => r.Id));
    }

    [Fact(DisplayName = "Given filters When listing products Then apply filters correctly")]
    public async Task Handle_WithFilters_AppliesFilters()
    {
        // Arrange
        var products = ProductHandlerTestData.GenerateListProducts(5);

        var queryable = products.AsQueryable();

        var command = new ListProductsQuery
        {
            Page = 1,
            Size = 5,
            Title = "Produto*",
            Description = "Descrição",
            Category = "Categoria",
            Image = "http",
            MinPrice = 10,
            MaxPrice = 100,
            Order = "Title desc"
        };

        _productRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _productRepository.Filter(Arg.Any<IQueryable<Product>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(callInfo => callInfo.Arg<IQueryable<Product>>());
        _productRepository.FilterRange(Arg.Any<IQueryable<Product>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(callInfo => callInfo.Arg<IQueryable<Product>>());
        _productRepository.OrderByFields(Arg.Any<IQueryable<Product>>(), Arg.Any<(string, bool)[]>()).Returns(callInfo => callInfo.Arg<IQueryable<Product>>());

        _mapper.ProjectTo<GetProductResult>(Arg.Any<IQueryable<Product>>()).Returns(products.Select(GetProductHandlerTestData.GenerateValidResult).AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _productRepository.Received(1).Filter(Arg.Any<IQueryable<Product>>(), nameof(GetProductResult.Title), command.Title);
        _productRepository.Received(1).Filter(Arg.Any<IQueryable<Product>>(), nameof(GetProductResult.Description), command.Description);
        _productRepository.Received(1).Filter(Arg.Any<IQueryable<Product>>(), nameof(GetProductResult.Category), command.Category);
        _productRepository.Received(1).Filter(Arg.Any<IQueryable<Product>>(), nameof(GetProductResult.Image), command.Image);
        _productRepository.Received(1).FilterRange(Arg.Any<IQueryable<Product>>(), nameof(GetProductResult.Price), command.MinPrice, command.MaxPrice);
        _productRepository.Received(1).OrderByFields(Arg.Any<IQueryable<Product>>(), Arg.Any<(string, bool)[]>());
        paginatedList.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given empty list When listing products Then return empty list")]
    public async Task Handle_EmptyList_ReturnsEmptyPaginatedList()
    {
        // Arrange
        var products = new List<Product>();
        var queryable = products.AsQueryable();

        var command = new ListProductsQuery
        {
            Page = 1,
            Size = 10
        };

        _productRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _mapper.ProjectTo<GetProductResult>(Arg.Any<IQueryable<Product>>()).Returns(new List<GetProductResult>().AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().BeEmpty();
    }
}