using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, Result>
    {
        public Task<Result> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
