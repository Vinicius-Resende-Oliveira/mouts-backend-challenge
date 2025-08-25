using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PaginatedList<GetUserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListUsersQueryHandler> _logger;

    public ListUsersQueryHandler(IUserRepository userRepository, IMapper mapper, ILogger<ListUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _logger.BeginScope("Begin ListUsersQueryHandler");
    }

    public async Task<PaginatedList<GetUserResult>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ListUsersQuery");
        var query = _userRepository.GetAll(cancellationToken).AsNoTracking();

        query = _userRepository.Filter(query, nameof(request.Phone), request.Phone);
        query = _userRepository.Filter(query, nameof(request.Username), request.Username);
        query = _userRepository.Filter(query, nameof(request.Email), request.Email);

        if (request.Role != UserRole.None)
            query = _userRepository.Filter(query, nameof(request.Role), request.Role.ToString());

        if (request.Status != UserStatus.Unknown)
            query = _userRepository.Filter(query, nameof(request.Status), request.Status.ToString());
        _logger.LogInformation("Filter query");

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
        _logger.LogInformation("Order query");

        var getUserList = _mapper.ProjectTo<GetUserResult>(query);
        _logger.LogInformation("Mapped GetUserResult");
        return ListUsersResponse.Create(getUserList, request.Page, request.Size);
    }
}
