using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class ProductsControllerTest : IClassFixture<CustomWebAppFactory>
{
	private readonly CustomWebAppFactory _factory;
	private readonly HttpClient _client;

	public ProductsControllerTest(CustomWebAppFactory factory)
	{
		_factory = factory;
		_client = _factory.CreateClient();
	}

	[Fact]
	public async Task CreateProduct_ShouldReturn201AndProductData()
	{
		var fakeProduct = ProductFakeData.GenerateValidProduct();
		var request = new CreateProductRequest
		{
			Title = fakeProduct.Title,
			Price = fakeProduct.Price,
			Description = fakeProduct.Description,
			Category = fakeProduct.Category,
			Image = fakeProduct.Image,
			Rating = new BaseRating(fakeProduct.Rating.Rate, fakeProduct.Rating.Count)
		};

		var response = await _client.PostAsJsonAsync("/api/products", request);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data.Should().NotBeNull();
		result.Data.Title.Should().Be(fakeProduct.Title);
	}

	[Fact]
	public async Task GetProduct_ShouldReturn200AndProductData()
	{
		// Arrange: create product first
		var fakeProduct = ProductFakeData.GenerateValidProduct();
		var createRequest = new CreateProductRequest
		{
			Title = fakeProduct.Title,
			Price = fakeProduct.Price,
			Description = fakeProduct.Description,
			Category = fakeProduct.Category,
			Image = fakeProduct.Image,
			Rating = new BaseRating(fakeProduct.Rating.Rate, fakeProduct.Rating.Count)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
		var productId = created?.Data?.Id ?? Guid.Empty;

		// Act
		var response = await _client.GetAsync($"/api/products/{productId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetProductResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(productId);
	}

	[Fact]
	public async Task UpdateProduct_ShouldReturn200AndUpdatedData()
	{
		// Arrange: create product first
		var fakeProduct = ProductFakeData.GenerateValidProduct();
		var createRequest = new CreateProductRequest
		{
			Title = fakeProduct.Title,
			Price = fakeProduct.Price,
			Description = fakeProduct.Description,
			Category = fakeProduct.Category,
			Image = fakeProduct.Image,
            Rating = new BaseRating(fakeProduct.Rating.Rate, fakeProduct.Rating.Count)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
		var productId = created?.Data?.Id ?? Guid.Empty;

		// Act: update product
		var updateRequest = new UpdateProductRequest
		{
			Id = productId,
			Title = "Updated Product",
			Price = 999.99m,
			Description = "Updated description",
			Category = "Updated Category",
			Image = "https://picsum.photos/200",
			Rating = new BaseRating(4.5, 100)
		};
		var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateRequest);
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateProductResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Title.Should().Be("Updated Product");
	}

	[Fact]
	public async Task DeleteProduct_ShouldReturn200()
	{
		// Arrange: create product first
		var fakeProduct = ProductFakeData.GenerateValidProduct();
		var createRequest = new CreateProductRequest
		{
			Title = fakeProduct.Title,
			Price = fakeProduct.Price,
			Description = fakeProduct.Description,
			Category = fakeProduct.Category,
			Image = fakeProduct.Image,
			Rating = new BaseRating(fakeProduct.Rating.Rate, fakeProduct.Rating.Count)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
		var productId = created?.Data?.Id ?? Guid.Empty;

		// Act: delete product
		var response = await _client.DeleteAsync($"/api/products/{productId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetAllProducts_ShouldReturnPaginatedList()
	{
		// Arrange: create some products
		for (int i = 0; i < 3; i++)
		{
			var fakeProduct = ProductFakeData.GenerateValidProduct();
			var createRequest = new CreateProductRequest
			{
				Title = fakeProduct.Title,
				Price = fakeProduct.Price,
				Description = fakeProduct.Description,
				Category = fakeProduct.Category,
				Image = fakeProduct.Image,
				Rating = new BaseRating(fakeProduct.Rating.Rate, fakeProduct.Rating.Count)
			};
			await _client.PostAsJsonAsync("/api/products", createRequest);
		}

		// Act
		var response = await _client.GetAsync("/api/products");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Count().Should().BeGreaterThanOrEqualTo(3);
	}
}
