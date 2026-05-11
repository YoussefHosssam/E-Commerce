using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.ResetUserPassword
{
    internal class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand, Result>
    {
        public Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
