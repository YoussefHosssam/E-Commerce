using E_Commerce.Application.Common.Result;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.DeleteUserAddress;

public sealed record DeleteUserAddressCommand(Guid AddressId) : IRequest<Result>;
