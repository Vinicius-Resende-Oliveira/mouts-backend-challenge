using Ambev.DeveloperEvaluation.Application.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Represents a request to update a user.
/// </summary>
public class UpdateUserRequest : BaseUser
{
    /// <summary>
    /// Gets or sets the unique identifier of the user to be updated.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the password for the user.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
