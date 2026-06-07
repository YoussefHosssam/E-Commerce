using E_Commerce.Application.Common.Result;
using MediatR;

namespace E_Commerce.Application.Features.Order.Commands.RetryOrderPayment;

public sealed record RetryOrderPaymentCommand(Guid OrderId) : IRequest<Result<PaymentDto>>;
