using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant
{
    public record UpdateStockMovementCommand(Guid productId , Guid VariantId,StockMovementType Type,int Quantity,string? Reason) : IRequest<Result>;
}
