using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new UpdateUserHandler(_userRepository, _mapper, _passwordHasher);
    }

    [Fact(DisplayName = "Handle should update user details when command is valid")]
    public async Task Handle_ShouldUpdateUserDetails_WhenCommandIsValid()
    {
        // Arrange
        var command = UpdateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = command.Id,
            Username = "oldusername",
            Email = "oldemail@example.com",
            Phone = "+5511987654321",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Name = new Name("Old", "Name"),
            Password = "OldPasswordHash"
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.HashPassword(command.Password).Returns("NewPasswordHash");
        _mapper.Map(command, user).Returns(user);
        _mapper.Map<UpdateUserResult>(user).Returns(UpdateUserHandlerTestData.GenerateValidResult(user.Id, command));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Username.Should().Be(command.Username);
        result.Email.Should().Be(command.Email);
        result.Phone.Should().Be(command.Phone);

        _userRepository.Received(1).Update(user);
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Handle should throw ValidationException when command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = UpdateUserHandlerTestData.GenerateInvalidCommand();

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Handle should throw KeyNotFoundException when user does not exist")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = UpdateUserHandlerTestData.GenerateValidCommand();

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with id {command.Id} not found");
    }
}
