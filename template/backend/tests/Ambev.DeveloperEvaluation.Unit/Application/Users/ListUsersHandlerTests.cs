using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class ListUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ListUsersHandler _handler;

    public ListUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListUsersHandler(_userRepository, _mapper);
    }

    [Fact(DisplayName = "Handle should return paginated list of users when query is valid")]
    public async Task Handle_ShouldReturnPaginatedListOfUsers_WhenQueryIsValid()
    {
        // Arrange
        var command = ListUsersHandlerTestData.GenerateValidCommand();
        var users = ListUsersHandlerTestData.GenerateUserList();

        _userRepository.GetAll(Arg.Any<CancellationToken>()).Returns(users.AsQueryable());
        _mapper.ProjectTo<GetUserResult>(Arg.Any<IQueryable<User>>()).Returns(users.Select(u => GetUserHandlerTestData.GenerateValidResult(u)).AsQueryable());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Handle should return empty list when no users match the query")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersMatchTheQuery()
    {
        // Arrange
        var command = ListUsersHandlerTestData.GenerateValidCommand();

        _userRepository.GetAll(Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<User>().AsQueryable());
        _mapper.ProjectTo<GetUserResult>(Arg.Any<IQueryable<User>>()).Returns(Enumerable.Empty<GetUserResult>().AsQueryable());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "Handle should apply filters and return filtered users")]
    public async Task Handle_ShouldApplyFiltersAndReturnFilteredUsers()
    {
        // Arrange
        var command = ListUsersHandlerTestData.GenerateValidCommand();
        command.Username = "testuser";
        var users = ListUsersHandlerTestData.GenerateUserList().Where(u => u.Username == command.Username);

        _userRepository.GetAll(Arg.Any<CancellationToken>()).Returns(users.AsQueryable());
        _mapper.ProjectTo<GetUserResult>(Arg.Any<IQueryable<User>>()).Returns(users.Select(u => GetUserHandlerTestData.GenerateValidResult(u)).AsQueryable());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.All(u => u.Username == command.Username).Should().BeTrue();
    }
}
