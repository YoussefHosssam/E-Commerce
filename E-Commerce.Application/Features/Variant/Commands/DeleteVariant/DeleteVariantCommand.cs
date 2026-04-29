using E_Commerce.Application.Common.Result;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.DeleteVariant;

public sealed record DeleteVariantCommand(Guid ProductId, Guid VariantId) : IRequest<Result>;

