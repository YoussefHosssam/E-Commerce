using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Payment.Commands
{
    public record PaymentWebhookCommand(PaymentGateway gateway , WebhookRequest webhook) : IRequest<Result>;
}
