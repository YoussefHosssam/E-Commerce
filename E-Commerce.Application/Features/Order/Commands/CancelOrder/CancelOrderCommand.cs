using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Order.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid id , string? reason) : IRequest<Result>;
}
