using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class CartsControllerTest : IClassFixture<CustomWebAppFactory>
{
	private readonly CustomWebAppFactory _factory;
	private readonly HttpClient _client;

	public CartsControllerTest(CustomWebAppFactory factory)
	{
		_factory = factory;
		_client = _factory.CreateClient();
	}

	[Fact]
	public async Task CreateCart_ShouldReturn201AndCartData()
	{
		var request = new CreateCartRequest
		{
			UserId = Guid.NewGuid(),
			Date = DateTime.UtcNow,
			Products = CartFakeData.GenerateCartItems(2)
		};

		var response = await _client.PostAsJsonAsync("/api/carts", request);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>();
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data.Should().NotBeNull();
		result.Data.Products.Should().NotBeNull();
		result.Data.Products!.Count.Should().Be(2);
	}

	[Fact]
	public async Task GetCart_ShouldReturn200AndCartData()
	{
		// Arrange: create cart first
		var createRequest = new CreateCartRequest
		{
			UserId = Guid.NewGuid(),
			Date = DateTime.UtcNow,
			Products = CartFakeData.GenerateCartItems(2)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/carts", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>();
		var cartId = created?.Data?.Id ?? Guid.Empty;

		// Act
		var response = await _client.GetAsync($"/api/carts/{cartId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetCartResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(cartId);
	}

	[Fact]
	public async Task UpdateCart_ShouldReturn200AndUpdatedData()
	{
		// Arrange: create cart first
		var createRequest = new CreateCartRequest
		{
			UserId = Guid.NewGuid(),
			Date = DateTime.UtcNow,
			Products = CartFakeData.GenerateCartItems(2)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/carts", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>();
		var cartId = created?.Data?.Id ?? Guid.Empty;

		// Act: update cart
		var updateRequest = new UpdateCartRequest
		{
			Id = cartId,
			UserId = createRequest.UserId,
			Date = DateTime.UtcNow.AddDays(1),
			Products = CartFakeData.GenerateCartItems(3)
		};
		var response = await _client.PutAsJsonAsync($"/api/carts/{cartId}", updateRequest);
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateCartResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Products.Should().NotBeNull();
		result.Data.Products!.Count.Should().Be(3);
	}

	[Fact]
	public async Task DeleteCart_ShouldReturn200()
	{
		// Arrange: create cart first
		var createRequest = new CreateCartRequest
		{
			UserId = Guid.NewGuid(),
			Date = DateTime.UtcNow,
			Products = CartFakeData.GenerateCartItems(2)
		};
		var createResponse = await _client.PostAsJsonAsync("/api/carts", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>();
		var cartId = created?.Data?.Id ?? Guid.Empty;

		// Act: delete cart
		var response = await _client.DeleteAsync($"/api/carts/{cartId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetAllCarts_ShouldReturnPaginatedList()
	{
		// Arrange: create some carts
		for (int i = 0; i < 3; i++)
		{
			var createRequest = new CreateCartRequest
			{
				UserId = Guid.NewGuid(),
				Date = DateTime.UtcNow,
				Products = CartFakeData.GenerateCartItems(2)
			};
			await _client.PostAsJsonAsync("/api/carts", createRequest);
		}

		// Act
		var response = await _client.GetAsync("/api/carts");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetCartResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Count().Should().BeGreaterThanOrEqualTo(3);
	}
}
