using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class UsersControllerTests : IClassFixture<CustomWebAppFactory>
{
	private readonly CustomWebAppFactory _factory;
	private readonly HttpClient _client;

	public UsersControllerTests(CustomWebAppFactory factory)
	{
		_factory = factory;
		_client = _factory.CreateClient();
	}

	[Fact]
	public async Task CreateUser_ShouldReturn201AndUserData()
	{
		var fakeUser = UserFakeData.GenerateValidUser();
		var request = new CreateUserRequest
		{
			Name = new BaseName(fakeUser.Name.FirstName, fakeUser.Name.LastName),
			Username = fakeUser.Username.Length > 50 ? fakeUser.Username[50..] : fakeUser.Username,
			Email = fakeUser.Email,
			Phone = fakeUser.Phone,
			Password = fakeUser.Password,
			Role = fakeUser.Role,
			Status = fakeUser.Status
		};

		var response = await _client.PostAsJsonAsync("/api/users", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
		result.Should().NotBeNull();
		result.Errors.Should().BeNullOrEmpty();
		result!.Success.Should().BeTrue();
		result.Data.Should().NotBeNull();
		result.Data.Email.Should().Be(fakeUser.Email);
	}

	[Fact]
	public async Task GetUser_ShouldReturn200AndUserData()
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
		var userId = created?.Data?.Id ?? Guid.Empty;

		// Act
		var response = await _client.GetAsync($"/api/users/{userId}");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetUserResponse>>();
        result.Should().NotBeNull();
        result.Errors.Should().BeNullOrEmpty();
		result!.Data.Should().NotBeNull();
		result.Data.Id.Should().Be(userId);
	}

	[Fact]
	public async Task UpdateUser_ShouldReturn200AndUpdatedData()
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
		var userId = created?.Data?.Id ?? Guid.Empty;

		// Act: update user
		var updateRequest = new UpdateUserRequest
		{
			Id = userId,
			Name = new BaseName("Updated", "User"),
			Username = "updateduser",
			Email = "updated@email.com",
			Phone = "11999999999",
            Password = fakeUser.Password,
            Role = fakeUser.Role,
            Status = fakeUser.Status
        };
		var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateUserResponse>>();
		result.Should().NotBeNull();
        result.Errors.Should().BeNullOrEmpty();
		result!.Data.Should().NotBeNull();
		result.Data.Email.Should().Be("updated@email.com");
    }

	[Fact]
	public async Task DeleteUser_ShouldReturn200()
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
		var userId = created?.Data?.Id ?? Guid.Empty;

		// Act: delete user
		var response = await _client.DeleteAsync($"/api/users/{userId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateUserResponse>>();
        result.Should().NotBeNull();
        result.Errors.Should().BeNullOrEmpty();
	}

	[Fact]
	public async Task GetAllUsers_ShouldReturnPaginatedList()
	{
		// Arrange: create some users
		for (int i = 0; i < 3; i++)
		{
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
        }

		// Act
		var response = await _client.GetAsync("/api/users");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetUserResponse>>();
		result.Should().NotBeNull();
        result.Errors.Should().BeNullOrEmpty();
        result!.Data.Should().NotBeNull();
		result.Data.Count().Should().BeGreaterThanOrEqualTo(3);
	}
}
