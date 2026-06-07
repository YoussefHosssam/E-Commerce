using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Results;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace E_Commerce.Application.Contracts.Infrastructure.Payment
{
    public interface IPaymentGateway
    {
        string Provider { get; }

        Task<ProviderResult<CreateProviderPaymentSessionResult>> CreateSessionAsync(
            CreateProviderPaymentSessionRequest request,
            CancellationToken ct);

        ProviderResult<ProviderWebhookEvent> ParseAndVerifyWebhook(
            WebhookRequest request);

    }
}
