using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers
{
    public class ListUsersRequest : PaginatedRequest
    {
        public string? Username { get; set; } = string.Empty;

        public string? Phone { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public UserStatus Status { get; set; }

        public UserRole Role { get; set; }
    }
}
