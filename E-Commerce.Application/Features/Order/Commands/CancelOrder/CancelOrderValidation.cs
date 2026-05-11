using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderValidation : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderValidation()
        {
            RuleFor(o => o.id)
                .NotEmpty()
                .WithError(OrderErrors.IdRequired);
        }
    }
}
