using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PaginatedList<GetUserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ListUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetUserResult>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userRepository.GetAll(cancellationToken).AsNoTracking();

        query = _userRepository.Filter(query, nameof(request.Phone), request.Phone);
        query = _userRepository.Filter(query, nameof(request.Username), request.Username);
        query = _userRepository.Filter(query, nameof(request.Email), request.Email);

        if (request.Role != UserRole.None)
            query = _userRepository.Filter(query, nameof(request.Role), request.Role.ToString());

        if (request.Status != UserStatus.Unknown)
            query = _userRepository.Filter(query, nameof(request.Status), request.Status.ToString());

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orders = request.Order
                .Trim().Trim('"', '\'')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(part =>
                {
                    var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = tokens[0];
                    var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                    return (field, desc);
                })
                .Where(t => !string.IsNullOrWhiteSpace(t.field))
                .ToArray();

            query = _userRepository.OrderByFields(query, orders);
        }
      
        var getUserList = _mapper.ProjectTo<GetUserResult>(query);
        return ListUsersResponse.Create(getUserList, request.Page, request.Size);
    }
}
