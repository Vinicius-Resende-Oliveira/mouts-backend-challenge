using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.Common;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers
{
    public class ListUsersProfile : Profile
    {
        public ListUsersProfile()
        {
            CreateMap<ListUsersRequest, ListUsersCommand>();
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>));
            CreateMap<GetUserResult, GetUserResponse>();
        }
    }
}
