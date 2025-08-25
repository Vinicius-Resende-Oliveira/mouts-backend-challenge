using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers
{
    public class ListUsersQuery : PaginatedListRequest, IRequest<PaginatedList<GetUserResult>>
    {
        /// <summary>
        /// The user's full name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The user's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's phone number
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// The user's status
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// The user's role
        /// </summary>
        public UserRole Role { get; set; }
    }
}
