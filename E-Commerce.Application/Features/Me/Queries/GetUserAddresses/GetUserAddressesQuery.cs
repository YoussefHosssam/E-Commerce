using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using MediatR;

namespace E_Commerce.Application.Features.Me.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery : IRequest<Result<IReadOnlyCollection<UserAddressDto>>>;
