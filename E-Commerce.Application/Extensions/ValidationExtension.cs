using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Extensions
{
    public static class ValidationExtension
    {
        public static IRuleBuilderOptions<T , Tprop> WithError<T, Tprop> (this IRuleBuilderOptions<T, Tprop> opt , string code)
        {
            return opt.WithErrorCode(code).WithMessage(ErrorCatalog.FromCode(code).Message);
        }
    }
}
