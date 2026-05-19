using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.SetDefaultUserAddress;

public sealed record SetDefaultUserAddressCommand(Guid AddressId) : IRequest<Result<UserAddressDto>>;
