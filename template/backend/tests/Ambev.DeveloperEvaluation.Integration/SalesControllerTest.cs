using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class SalesControllerTest : IClassFixture<CustomWebAppFactory>
{
	private readonly CustomWebAppFactory _factory;
	private readonly HttpClient _client;
	private Guid _productId = Guid.Empty;

    public SalesControllerTest(CustomWebAppFactory factory)
	{
		_factory = factory;
		_client = _factory.CreateClient();
	}

	private async Task<CreateSaleRequest> GenerateValidSaleRequest(Guid? cartId = null)
    {
        var userId = await GetUser();
        var productId = await GetProduct();
		_productId = productId;
        var cart = await GetCart(productId, userId);
        cartId ??= cart?.Id ?? Guid.Empty;

        var sale = SaleFakeData.GenerateValidSale();
		sale.CartId = cartId.Value;
        return sale;
    }

	private async Task<Guid> GetUser()
	{
		// Arrange: create user first
		var fakeUser = UserFakeData.GenerateValidUser();
		var createRequest = new CreateUserRequest
		{
			Name = new BaseName(fakeUser.Name.FirstName, fakeUser.Name.LastName),
			Username = fakeUser.Username.Length > 50 ? fakeUser.Username[50..] : fakeUser.Username,
			Email = fakeUser.Email,
			Phone = fakeUser.Phone,
			Password = fakeUser.Password,
			Role = fakeUser.Role,
			Status = fakeUser.Status
		};
		var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
        return created?.Data?.Id ?? Guid.Empty;
    }

    private async Task<Guid> GetProduct()
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

        var createResponse = await _client.PostAsJsonAsync("/api/products", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
        return created?.Data?.Id ?? Guid.Empty;
    }

    private async Task<CreateCartResponse?> GetCart(Guid productId, Guid userId)
    {
		var product = CartFakeData.GenerateValidCartItem(productId);
        var createRequest = new CreateCartRequest
        {
            UserId = userId,
            Date = DateTime.UtcNow,
            Products = new()
			{
				product 
			}
        };
        var createResponse = await _client.PostAsJsonAsync("/api/carts", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>();
        return created?.Data;
    }

    [Fact]
	public async Task CreateSale_ShouldReturn201AndSaleData()
	{
		var request = await GenerateValidSaleRequest();

		var response = await _client.PostAsJsonAsync("/api/sales", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data.Should().NotBeNull();
		//result.Data.Customer.Should().Be(request.Customer);
	}

	[Fact]
	public async Task GetSale_ShouldReturn200AndSaleData()
	{
		// Arrange: create sale first
		var createRequest = await GenerateValidSaleRequest();
		var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
		var saleId = created?.Data?.Id ?? Guid.Empty;

		// Act
		var response = await _client.GetAsync($"/api/sales/{saleId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(saleId);
	}

	[Fact]
	public async Task UpdateSale_ShouldReturn200AndUpdatedData()
	{
		// Arrange: create sale first
		var createRequest = await GenerateValidSaleRequest();
		var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = created?.Data?.Id ?? Guid.Empty;

        // Act: update sale
        var updateRequest = new UpdateSaleRequest
		{
			Id = saleId,
			SaleDate = createRequest.SaleDate.AddDays(1),
			Customer = "Updated Customer",
			Branch = "Updated Branch",
            Items = new List<UpdateSaleItemRequest>()
			{
				new ()
				{
					ProductId = _productId,
					Quantity = 10
				}
			}
        };
		var response = await _client.PutAsJsonAsync($"/api/sales/{saleId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Customer.Should().Be("Updated Customer");
	}

	[Fact]
	public async Task DeleteSale_ShouldReturn200()
	{
		// Arrange: create sale first
		var createRequest = await GenerateValidSaleRequest();
		var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);

        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
		var saleId = created?.Data?.Id ?? Guid.Empty;

		// Act: delete sale
		var response = await _client.DeleteAsync($"/api/sales/{saleId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetAllSales_ShouldReturnPaginatedList()
	{
		// Arrange: create some sales
		for (int i = 0; i < 3; i++)
		{
			var createRequest = await GenerateValidSaleRequest();
			var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);

        }

		// Act
		var response = await _client.GetAsync("/api/sales");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetSaleResponse>>();
		result.Should().NotBeNull();
		result!.Data.Should().NotBeNull();
		result.Data.Count().Should().BeGreaterThanOrEqualTo(3);
	}
}
