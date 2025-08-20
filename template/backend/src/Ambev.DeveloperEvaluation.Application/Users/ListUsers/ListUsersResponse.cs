using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersResponse : PaginatedList<GetUserResult>
{
    public ListUsersResponse(List<GetUserResult> items, int count, int pageNumber, int pageSize)
        : base(items, count, pageNumber, pageSize)
    {
    }
}
