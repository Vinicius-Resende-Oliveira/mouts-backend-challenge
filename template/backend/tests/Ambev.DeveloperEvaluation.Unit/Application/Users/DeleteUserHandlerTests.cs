using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new DeleteUserHandler(_userRepository);
    }

    [Fact(DisplayName = "Given a valid command When deleting user Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var command = DeleteUserHandlerTestData.GenerateValidCommand();
        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _userRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When deleting user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = DeleteUserHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a non-existent user When deleting user Then throws not found exception")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = DeleteUserHandlerTestData.GenerateValidCommand();
        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {command.Id} not found");
    }
}
