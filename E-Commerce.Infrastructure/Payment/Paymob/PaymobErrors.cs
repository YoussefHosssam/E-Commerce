using E_Commerce.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Payment.Paymob
{
    internal static class PaymobErrors
    {
        public static readonly Error CancelRequest =
            new(
                ErrorCodes.Paymob.CancelRequest,
                "The Paymob request was cancelled.",
                ErrorType.Failure);

        public static readonly Error FailedRequest =
            new(
                ErrorCodes.Paymob.FailedRequest,
                "Paymob request failed.",
                ErrorType.Failure);
    }
}
