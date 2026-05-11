using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.ForgetUserPassword
{
    public record ForgetUserPasswordCommand : IRequest<Result>
    {
    }
}
