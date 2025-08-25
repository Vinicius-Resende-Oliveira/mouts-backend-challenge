using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserQueryHandler> _logger;
    private readonly GetUserQueryHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetUserQueryHandler>>();
        _handler = new GetUserQueryHandler(_userRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Handle should return user details when user exists")]
    public async Task Handle_ShouldReturnUserDetails_WhenUserExists()
    {
        // Arrange
        var command = GetUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = command.Id,
            Username = "testuser",
            Email = "testuser@example.com",
            Phone = "+5511999999999",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Name = new Name("Test", "User")
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(GetUserHandlerTestData.GenerateValidResult(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Handle should throw ValidationException when command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = GetUserHandlerTestData.GenerateInvalidCommand();

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Handle should throw KeyNotFoundException when user does not exist")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = GetUserHandlerTestData.GenerateValidCommand();

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {command.Id} not found");
    }
}
