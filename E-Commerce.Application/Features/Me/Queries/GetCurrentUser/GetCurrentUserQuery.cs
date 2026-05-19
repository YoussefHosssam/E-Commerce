using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using MediatR;

namespace E_Commerce.Application.Features.Me.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>;
