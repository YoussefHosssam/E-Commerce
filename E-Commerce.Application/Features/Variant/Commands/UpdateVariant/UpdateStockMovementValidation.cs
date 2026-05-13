using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant
{
    internal class UpdateStockMovementValidation : AbstractValidator<UpdateStockMovementCommand>
    {
        public UpdateStockMovementValidation()
        {
            RuleFor(x => x.productId).NotEmpty().WithError(ProductErrors.InvalidInput);
            RuleFor(x => x.VariantId).NotEmpty().WithError(StockMovementErrors.VariantIdRequired);
            RuleFor(x => x.Type).IsInEnum().WithError(StockMovementErrors.TypeInvalid);
            RuleFor(x => x.Reason).Length(0 , 500).WithError(StockMovementErrors.TypeInvalid);
            RuleFor(x => x.Quantity).NotEmpty().WithError(StockMovementErrors.QuantityDeltaInvalid);
        }
    }
}
