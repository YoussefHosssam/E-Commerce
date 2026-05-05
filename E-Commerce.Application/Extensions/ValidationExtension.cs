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
        public static IRuleBuilderOptions<T , Tprop> WithError<T, Tprop> (this IRuleBuilderOptions<T, Tprop> opt , Error err)
        {
            return opt.WithErrorCode(err.Code).WithMessage(err.Message);
        }
    }
}
