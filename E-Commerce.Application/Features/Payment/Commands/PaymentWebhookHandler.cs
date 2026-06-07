using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Domain.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Payment.Commands
{
    public class PaymentWebhookHandler : IRequestHandler<PaymentWebhookCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentWebhookHandler(IUnitOfWork uow, IPaymentGateway paymentGateway)
        {
            _uow = uow;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result> Handle(
    PaymentWebhookCommand request,
    CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var rawPayload = JsonText.Create(request.webhook.rawBody);

            var providerResponseResult =_paymentGateway.ParseAndVerifyWebhook(request.webhook);

            if (!providerResponseResult.IsSuccess)
                return Result.Fail(PaymentErrors.FailParsingWebhook);

            var providerResponse = providerResponseResult.Data!;

            if (string.IsNullOrWhiteSpace(providerResponse.ProviderOrderId))
                return Result.Fail(PaymentErrors.InvalidOrderId);

            var paymentAttempt =
                await _uow.PaymentAttempts.GetPaymentAttemptByProviderOrderIdAsync(
                    providerResponse.ProviderOrderId,
                    cancellationToken);

            if (paymentAttempt is null)
                return Result.Fail(PaymentErrors.InvalidOrderId);

            var order =
                await _uow.Orders.GetTrackingOrderByIdWithDetailsAsync(
                    paymentAttempt.OrderId,
                    cancellationToken);

            if (order is null)
                return Result.Fail(PaymentErrors.InvalidOrderId);

            if (paymentAttempt.IsPaid || order.IsPaid)
                return Result.Success();

            if (providerResponse.Pending)
                return Result.Success();

            if (providerResponse.Amount != order.GrandTotal ||
                providerResponse.Currency != order.Currency)
            {
                paymentAttempt.MarkFailed(now, rawPayload);
                await _uow.SaveChangesAsync(cancellationToken);

                return Result.Fail(PaymentErrors.InvalidPaymentAmount);
            }

            if (providerResponse.Success)
            {
                paymentAttempt.MarkPaid(now, rawPayload);
                order.MarkPaid(now);

                var payment = Domain.Entities.Payment.Create(
                    order.Id,
                    providerResponse.Provider,
                    order.GrandTotal,
                    order.Currency,
                    rawPayload,
                    now);
                payment.AttachProviderPaymentId(providerResponse.ProviderSessionId! , now);
                payment.MarkSuccessed(now);
                await _uow.Payments.CreateAsync(payment, cancellationToken);
            }
            else
            {
                paymentAttempt.MarkFailed(now, rawPayload);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
